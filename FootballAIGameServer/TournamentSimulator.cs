using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.ApiForWeb;
using FootballAIGameServer.Models;

namespace FootballAIGameServer
{
    public class TournamentSimulator
    {
        private ApplicationDbContext _context;

        private ConnectionManager ConnectionManager { get; set; }
        private int TournamentId { get; set; }
        private DateTime StartTime { get; set; }
        private List<TournamentPlayer> Players { get; set; }

        private TimeSpan TimeUntilStart => StartTime - DateTime.Now;

        public static List<TournamentSimulator> RunningTournaments { get; set; }
            = new List<TournamentSimulator>();

        public static void PlanNextTournaments()
        {
            using (var context = new ApplicationDbContext())
            {
                var nextTournaments = context.Tournaments
                    .Where(t => t.TournamentState == TournamentState.Unstarted)
                    .ToList();

                foreach (var nextTournament in nextTournaments)
                {
                    var simulator = new TournamentSimulator(ConnectionManager.Instance, nextTournament);
                    simulator.PlanSimulation();
                }
            }
        }

        public TournamentSimulator(ConnectionManager connectionManager, Tournament tournament)
        {
            ConnectionManager = connectionManager;
            TournamentId = tournament.Id;
            StartTime = tournament.StartTime;

            _context = new ApplicationDbContext();
        }

        public async Task PlanSimulation()
        {
            Console.WriteLine($"Simulation of tournament {TournamentId} is being planned!");
            await Task.Delay(10000);


            // sleep until 5 minutes before tournament
            if (TimeUntilStart.TotalMinutes > 5)
            {
                //await Task.Delay(TimeUntilStart - TimeSpan.FromMinutes(5));
                Console.WriteLine($"Simulation of tournament {TournamentId} is awaken 5 minutes before start!");
                KickInactive();
            }

            // when tournament starts
            //if (TimeUntilStart.TotalSeconds > 0)
            //await Task.Delay(TimeUntilStart);

            Console.WriteLine($"Simulation of tournament {TournamentId} is being simulated!");
            KickInactive();
            await Simulate();
            Console.WriteLine($"Simulation of tournament {TournamentId} ends!");
        }

        /// <summary>
        /// Player will leave a running tournament in which he currently is.
        /// If there is not such tournament, then it does nothing.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        public static void LeaveRunningTournament(string playerName)
        {
            var tournament = RunningTournaments
                .SingleOrDefault(t => t.Players.Any(p => p.Player.Name == playerName));
            if (tournament == null)
                return;

            tournament.Players.RemoveAll(p => p.Player.Name == playerName);

            // improve position for players that have already ended
            using (var context = new ApplicationDbContext())
            {
                var playersWithPosition = context.TournamentPlayers
                    .Where(tp => tp.TournamentId == tournament.TournamentId && tp.PlayerPosition != null);
                foreach (var tournamentPlayer in playersWithPosition)
                {
                    tournamentPlayer.PlayerPosition--;
                }
                context.SaveChanges();
            }
        }

        private async Task Simulate()
        {
            Tournament tournament;

            using (var context = new ApplicationDbContext())
            {
                // get tournament
                tournament = context.Tournaments
                    .Include(t => t.Players.Select(tp => tp.Player))
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
                    context.SaveChanges();
                    return;
                }

                tournament.TournamentState = TournamentState.Running;
                RunningTournaments.Add(this);

                // get players and update their states
                Players = new List<TournamentPlayer>(); // players in tournament
                foreach (var tournamentPlayer in tournament.Players)
                    if (tournamentPlayer.Player.PlayerState == PlayerState.Idle)
                    {
                        tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentWaiting;
                        Players.Add(tournamentPlayer);
                    }

                context.SaveChanges();
            }

            // while there is more than 1 player -> round simulation
            while (Players.Count > 1)
            {
                Players = await SimulateRound(tournament);
            }

            if (Players.Count == 1)
            {
                var player = Players.First();
                player.PlayerPosition = 1;
                player.Player.PlayerState = PlayerState.Idle;
                SavePlayers(Players);
            }

            // set state to finished
            using (var context = new ApplicationDbContext())
            {
                tournament = context.Tournaments.Single(t => t.Id == TournamentId);
                tournament.TournamentState = TournamentState.Finished;
                context.SaveChanges();
            }
        }

        private async Task<List<TournamentPlayer>> SimulateRound(Tournament tournament)
        {
            Console.WriteLine($"Tournament {TournamentId} : Simulating round.");

            var advancingPlayers = new List<TournamentPlayer>();

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
                Players.RemoveAll(p => advancingPlayers.Contains(p));
            }

            TournamentPlayer firstPlayer = null;
            var matches = new List<MatchSimulator>();

            foreach (var tournamentPlayer in Players)
            {
                if (firstPlayer == null)
                    firstPlayer = tournamentPlayer;
                else // start new match
                {
                    SimulationManager.StartMatch(firstPlayer.Player.Name, firstPlayer.PlayerAi,
                        tournamentPlayer.Player.Name, tournamentPlayer.PlayerAi, TournamentId);
                    // update player states
                    firstPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;
                    tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;
                    // add match to matches
                    matches.Add(SimulationManager.GetMatchSimulator(firstPlayer.Player.Name));
                    firstPlayer = null;
                }
            }

            SavePlayers(Players); // update states

            await Task.WhenAll(matches.Select(m => m.CurrentSimulationTask).ToArray());

            Players.ForEach(p => p.Player.PlayerState = PlayerState.PlayingTournamentWaiting);

            // get looser position number
            var exp = 1;
            while (exp*2 < Players.Count)
                exp *= 2;
            var looserPos = exp + 1; // ex. from 8. (2^3) to 5. (2^2+1) player they will have position 5

            foreach (var matchSimulator in matches)
            {
                TournamentPlayer winner, looser;

                if (matchSimulator.Winner != null)
                {
                    winner = Players.FirstOrDefault(p => p.Player.Name == matchSimulator.Winner);
                    looser = matchSimulator.Player1AiConnection.PlayerName == matchSimulator.Winner
                        ? Players.FirstOrDefault(p => p.Player.Name == matchSimulator.Player2AiConnection.PlayerName)
                        : Players.FirstOrDefault(p => p.Player.Name == matchSimulator.Player1AiConnection.PlayerName);

                }
                else
                {
                    var p1 = Players.FirstOrDefault(p => p.Player.Name == matchSimulator.Player1AiConnection.PlayerName);
                    var p2 = Players.FirstOrDefault(p => p.Player.Name == matchSimulator.Player2AiConnection.PlayerName);
                    var rndWinner = MatchSimulator.Random.Next(2);
                    winner = rndWinner == 0 ? p1 : p2;
                    looser = rndWinner == 0 ? p2 : p1;
                }

                if (winner == null && looser != null)
                {
                    winner = looser;
                    looser = null;
                }
                
                if (winner != null)
                    advancingPlayers.Add(winner);
                if (looser == null) continue;
                looser.PlayerPosition = looserPos;
                looser.Player.PlayerState = PlayerState.Idle;
            }

            SavePlayers(Players);
            advancingPlayers.RemoveAll(p => !Players.Contains(p)); // remove all skipping players that left during round
            return advancingPlayers;
        }

        /// <summary>
        /// Saves players states and tournament positions.
        /// </summary>
        /// <param name="players">The players.</param>
        private void SavePlayers(IEnumerable<TournamentPlayer> players)
        {
            using (var context = new ApplicationDbContext())
            {
                var dbPlayers = context.TournamentPlayers
                    .Include(tp => tp.Player)
                    .Where(t => t.TournamentId == TournamentId);

                foreach (var player in players)
                {
                    var dbPlayer = dbPlayers.Single(p => p.UserId == player.UserId);
                    dbPlayer.Player.PlayerState = player.Player.PlayerState;
                    dbPlayer.PlayerPosition = player.PlayerPosition;
                }
                context.SaveChanges();
            }
        }

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
                    if (tournamentPlayer.Player.ActiveAis == null)
                    {
                        playersToBeRemoved.Add(tournamentPlayer);
                        continue;
                    }
                    var activeAis = tournamentPlayer.Player.ActiveAis.Split(';');
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

        public static void CloseRunningTournaments(ApplicationDbContext context)
        {
            var runningTournaments = context.Tournaments.Where(t => t.TournamentState == TournamentState.Running);
            foreach (var runningTournament in runningTournaments)
            {
                runningTournament.TournamentState = TournamentState.ErrorClosed;
            }
        }

    }
}
