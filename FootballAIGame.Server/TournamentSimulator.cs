using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation;
using FootballAIGame.MatchSimulation.Models;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Server
{
    /// <summary>
    /// Provides the functionality to simulate a tournament.
    /// </summary>
    class TournamentSimulator
    {
        /// <summary>
        /// Gets or sets the tournament's ID of the tournament that
        /// is simulated by this instance.
        /// </summary>
        /// <value>
        /// The tournament's ID.
        /// </value>
        public int TournamentId { get; }

        /// <summary>
        /// Gets or sets the start time of the tournament.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        private DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Random" /> used for generating random numbers.
        /// </summary>
        /// <value>
        /// The <see cref="Random"/> instance.
        /// </value>
        public Random Random { get; set; } = new Random();

        /// <summary>
        /// Gets or sets the players that are currently in the running tournament. It is used for
        /// simulation and is empty if the tournament is not currently running.
        /// </summary>
        /// <value>
        /// The list of players that are currently in the running tournament.
        /// </value>
        public List<TournamentPlayer> Players { get; set; }

        /// <summary>
        /// Gets the time until the start of the tournament.
        /// </summary>
        /// <value>
        /// The time until the tournament's start.
        /// </value>
        private TimeSpan TimeUntilStart => StartTime - DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSimulator"/> class.
        /// </summary>
        /// <param name="tournament">The tournament for simulation.</param>
        public TournamentSimulator(Tournament tournament)
        {
            TournamentId = tournament.Id;
            StartTime = tournament.StartTime;
        }

        /// <summary>
        /// Plans the simulation.
        /// </summary>
        public void PlanSimulation()
        {
            Console.WriteLine($"Simulation of tournament {TournamentId} is being planned!");

            Task.Run(async () =>
            {
                // sleep until 5 minutes before tournament
                if (TimeUntilStart.TotalMinutes > 5)
                {
                    await Task.Delay(TimeUntilStart - TimeSpan.FromMinutes(5));
                    Console.WriteLine($"Simulation of tournament {TournamentId} is awaken 5 minutes before start!");
                    KickInactive();
                }

                // when tournament starts
                if (TimeUntilStart.TotalSeconds > 0)
                    await Task.Delay(TimeUntilStart);

                Console.WriteLine($"Simulation of tournament {TournamentId} is being simulated!");
                KickInactive();
                await SimulateAsync();
                Console.WriteLine($"Simulation of tournament {TournamentId} ends!");
            });
        }

        /// <summary>
        /// Simulated the tournament asynchronously.
        /// </summary>
        /// <returns>The task that represents the asynchronous simulate operation.</returns>
        private async Task SimulateAsync()
        {
            Tournament tournament;

            lock (TournamentManager.Instance.RunningTournaments) // only one simultaneous starting
            {
                using (var context = new ApplicationDbContext())
                {
                    // get tournament
                    tournament = context.Tournaments
                       .Include(t => t.Players.Select(tp => tp.Player))
                       .Include(t => t.RecurringTournament)
                       .SingleOrDefault(t => t.Id == TournamentId);

                    if (tournament == null)
                    {
                        Console.WriteLine($"ERROR: Cannot find tournament with id {TournamentId} in database.");
                        return;
                    }

                    // set state -> check if there is enough players
                    if (tournament.Players.Count < tournament.MinimumNumberOfPlayers)
                    {
                        tournament.TournamentState = TournamentState.NotEnoughtPlayersClosed;

                        if (tournament.RecurringTournament != null)
                        {
                            TournamentManager.PlanNextRecurring(tournament.RecurringTournament.Id);
                        }

                        context.SaveChanges();
                        return;
                    }

                    tournament.TournamentState = TournamentState.Running;

                    TournamentManager.Instance.RunningTournaments.Add(this);

                    // get players and update their states
                    Players = new List<TournamentPlayer>(); // players in tournament
                    lock (Players)
                    {
                        var toBeRemovedTournamentPlayers = new List<TournamentPlayer>();

                        foreach (var tournamentPlayer in tournament.Players)
                        {
                            if (tournamentPlayer.Player.PlayerState == PlayerState.Idle)
                            {
                                Players.Add(tournamentPlayer);
                            }
                            else
                            {
                                toBeRemovedTournamentPlayers.Add(tournamentPlayer);
                            }
                        }

                        context.TournamentPlayers.RemoveRange(toBeRemovedTournamentPlayers); // disqualified

                        // check again after the remove of toBeRemovedTournamentPlayers
                        if (Players.Count < tournament.MinimumNumberOfPlayers)
                        {
                            tournament.TournamentState = TournamentState.NotEnoughtPlayersClosed;

                            if (tournament.RecurringTournament != null)
                                TournamentManager.PlanNextRecurring(tournament.RecurringTournament.Id);

                            context.SaveChanges();
                            return;
                        }

                        foreach (var tournamentPlayer in Players)
                            tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentWaiting;
                    }

                    context.SaveChanges();
                }
            }

            // while there is more than 1 player -> round simulation
            int n;
            lock (Players)
            {
                n = Players.Count;
            }

            while (n > 1)
            {
                Players = await SimulateRoundAsync(tournament);
                lock (Players)
                {
                    n = Players.Count;
                }
            }

            if (n == 1)
            {
                TournamentPlayer player;
                lock (Players)
                {
                    player = Players.FirstOrDefault();
                }

                if (player != null)
                {
                    player.PlayerPosition = 1;
                    player.Player.Score += 3;
                    player.Player.WonTournaments++;
                    player.Player.PlayerState = PlayerState.Idle;
                }

                await SavePlayersAsync(Players);
            }

            // set state to finished
            using (var context = new ApplicationDbContext())
            {
                tournament = context.Tournaments
                    .Include(t => t.RecurringTournament)
                    .Single(t => t.Id == TournamentId);
                tournament.TournamentState = TournamentState.Finished;
                context.SaveChanges();
            }

            // if it's recurring then plan new one
            if (tournament.RecurringTournament != null)
            {
                TournamentManager.PlanNextRecurring(tournament.RecurringTournament.Id);
            }
        }

        /// <summary>
        /// Simulates the tournament's round. Returns the list of advancing players.
        /// </summary>
        /// <param name="tournament">The tournament.</param>
        /// <returns>The task that represents the asynchronous simulate operation.
        /// The value of the task's result is the list of advancing players.</returns>
        private async Task<List<TournamentPlayer>> SimulateRoundAsync(Tournament tournament)
        {
            Console.WriteLine($"Tournament {TournamentId} : Simulating round.");

            var advancingPlayers = new List<TournamentPlayer>();
            var fightingPlayers = new List<TournamentPlayer>();
            var simulationTasks = new List<Task<MatchSimulator>>();

            lock (Players)
            {
                // first round byes (some players may proceed directly to second round 
                // if number of players is not a power of two)
                var isNumOfPlayersTwoPower = (Players.Count & (Players.Count - 1)) == 0;
                if (!isNumOfPlayersTwoPower)
                {
                    var nextPowerOfTwo = tournament.Players.Count;
                    while ((nextPowerOfTwo & (nextPowerOfTwo - 1)) != 0) // while it's not a power of two
                        nextPowerOfTwo++;

                    // number of players without opponent (best will be chosen)
                    var numOfSkippingPlayers = nextPowerOfTwo - Players.Count;

                    var skippingPlayers = Players
                        .OrderByDescending(p => p.Player.Score)
                        .Take(numOfSkippingPlayers);

                    advancingPlayers.AddRange(skippingPlayers);
                }

                fightingPlayers.AddRange(Players.Where(p => !advancingPlayers.Contains(p)));

                TournamentPlayer firstPlayer = null;

                foreach (var tournamentPlayer in fightingPlayers)
                {
                    if (firstPlayer == null)
                        firstPlayer = tournamentPlayer;
                    else // start new match
                    {
                        Task<MatchSimulator> simulationTask;

                        SimulationManager.Instance.StartMatch(firstPlayer.Player.Name, firstPlayer.PlayerAi,
                            tournamentPlayer.Player.Name, tournamentPlayer.PlayerAi, out simulationTask, tournament.Id);

                        // update player states
                        firstPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;
                        tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;

                        // add match to matches
                        simulationTasks.Add(simulationTask);
                        firstPlayer = null;
                    }
                }
            }

            await SavePlayersAsync(fightingPlayers); // update states

            await Task.WhenAll(simulationTasks);

            var matches = new List<MatchSimulator>();
            foreach (var simulationTask in simulationTasks)
            {
                matches.Add(await simulationTask);
            }

            lock (Players)
            {
                Players.ForEach(p => p.Player.PlayerState = PlayerState.PlayingTournamentWaiting);

                // get looser position number
                var exp = 1;
                while (exp * 2 < Players.Count)
                    exp *= 2;
                var looserPos = exp + 1; // ex. from 8. (2^3) to 5. (2^2+1) player they will have position 5

                foreach (var matchSimulator in matches)
                {
                    TournamentPlayer winner, looser;

                    if (matchSimulator.MatchInfo.Winner != null)
                    {
                        string winnerPlayerName;
                        string looserPlayerName;

                        if (matchSimulator.MatchInfo.Winner == Team.FirstPlayer)
                        {
                            winnerPlayerName = matchSimulator.AI1Communicator.PlayerName;
                            looserPlayerName = matchSimulator.AI2Communicator.PlayerName;
                        }
                        else
                        {
                            winnerPlayerName = matchSimulator.AI2Communicator.PlayerName;
                            looserPlayerName = matchSimulator.AI1Communicator.PlayerName;
                        }


                        winner = Players.FirstOrDefault(p => p.Player.Name == winnerPlayerName);
                        looser = Players.FirstOrDefault(p => p.Player.Name == looserPlayerName);
                    }
                    else
                    {
                        var player1 =
                            Players.FirstOrDefault(p => p.Player.Name == matchSimulator.AI1Communicator.PlayerName);
                        var player2 =
                            Players.FirstOrDefault(p => p.Player.Name == matchSimulator.AI2Communicator.PlayerName);
                        var rndWinner = Random.Next(2);

                        // in tournament winner is chosen randomly in case of draw !
                        winner = rndWinner == 0 ? player1 : player2;
                        looser = rndWinner == 0 ? player2 : player1;
                    }

                    if (winner == null && looser != null)
                    {
                        winner = looser;
                        looser = null;
                    }

                    if (winner != null)
                        advancingPlayers.Add(winner);
                    if (looser == null)
                        continue;
                    looser.PlayerPosition = looserPos;
                    looser.Player.PlayerState = PlayerState.Idle;

                    // increase looser score in accordance with his position
                    if (looserPos <= 3)
                        looser.Player.Score += 4 - looserPos;
                }

                // remove all skipping players that left during round
                // don't save those that have already left
                fightingPlayers.RemoveAll(p => !Players.Contains(p));
            }

            await SavePlayersAsync(fightingPlayers);

            return advancingPlayers;
        }

        /// <summary>
        /// Saves players' states and tournament's positions.
        /// </summary>
        /// <param name="players">The players to be saved.</param>
        private async Task SavePlayersAsync(IEnumerable<TournamentPlayer> players)
        {
            using (var context = new ApplicationDbContext())
            {
                var dbPlayers = await context.TournamentPlayers
                    .Include(tp => tp.Player)
                    .Where(t => t.TournamentId == TournamentId).ToListAsync();

                lock (Players)
                {
                    foreach (var player in players)
                    {
                        var dbPlayer = dbPlayers.Single(p => p.UserId == player.UserId);
                        dbPlayer.Player.PlayerState = player.Player.PlayerState;
                        dbPlayer.PlayerPosition = player.PlayerPosition;
                        dbPlayer.Player.WonTournaments = player.Player.WonTournaments;
                        dbPlayer.Player.Score = player.Player.Score;
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Kicks inactive players from the tournament.
        /// </summary>
        private void KickInactive()
        {
            using (var context = new ApplicationDbContext())
            {
                var tournamentPlayers = context.TournamentPlayers
                    .Include(tp => tp.Player)
                    .Where(tp => tp.TournamentId == TournamentId)
                    .ToList();
                var playersToBeRemoved = new List<TournamentPlayer>();

                foreach (var tournamentPlayer in tournamentPlayers)
                {
                    if (tournamentPlayer.Player.ActiveAIs == null)
                    {
                        playersToBeRemoved.Add(tournamentPlayer);
                        continue;
                    }
                    var activeAis = tournamentPlayer.Player.ActiveAIs.Split(';');
                    if (!activeAis.Contains(tournamentPlayer.PlayerAi))
                        playersToBeRemoved.Add(tournamentPlayer);
                }

                foreach (var tournamentPlayer in playersToBeRemoved)
                {
                    Console.WriteLine($"Removing {tournamentPlayer.Player.Name} from tournament " +
                                        $"{TournamentId}, because he doesn't have {tournamentPlayer.PlayerAi} active!");
                }

                context.TournamentPlayers.RemoveRange(playersToBeRemoved);
                context.SaveChanges();
            }
        }

    }
}
