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
            Console.WriteLine($"Simulation {TournamentId} is being planned!");

            // sleep until 5 minutes before tournament
            if (TimeUntilStart.TotalMinutes > 5)
            {
                //await Task.Delay(TimeUntilStart - TimeSpan.FromMinutes(5));
                Console.WriteLine($"Simulation {TournamentId} is awaken 5 minutes before start!");
                KickInactive();
            }

            // when tournament starts
            //if (TimeUntilStart.TotalSeconds > 0)
                //await Task.Delay(TimeUntilStart);
            await Task.Delay(10000);

            Console.WriteLine($"Simulation {TournamentId} is being simulated!");
            KickInactive();
            await Simulate();
        }

        private async Task Simulate()
        {
            Tournament tournament;
            List<TournamentPlayer> players;

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
                players = new List<TournamentPlayer>(); // players in tournament
                foreach (var tournamentPlayer in tournament.Players)
                    if (tournamentPlayer.Player.PlayerState == PlayerState.Idle)
                    {
                        tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentWaiting;
                        players.Add(tournamentPlayer);
                    }

                context.SaveChanges();
            }

            // while there is more than 0 player -> round simulation
                while (players.Count > 0)
                    await SimulateRound(players, tournament);

                // set state to finished



            
        }

        private async Task SimulateRound(List<TournamentPlayer> players, Tournament tournament)
        {
            Console.WriteLine($"Tournament {TournamentId} : Simulating round.");

            var advancingPlayers = new List<TournamentPlayer>();

            // first round byes (some players may proceed directly to second round 
            // if number of players is not a power of two)
            var isNumOfPlayersTwoPower = (players.Count & (players.Count - 1)) == 0;
            if (!isNumOfPlayersTwoPower)
            {
                var nextPowerOfTwo = tournament.Players.Count;
                while ((nextPowerOfTwo & (nextPowerOfTwo - 1)) != 0) // while it's not a power of two
                    nextPowerOfTwo++;

                // number of players without opponent (best will be chosen)
                var numOfSkippingPlayers = nextPowerOfTwo - players.Count;

                var skippingPlayers = players
                    .OrderByDescending(p => p.Player.Score)
                    .Take(numOfSkippingPlayers);

                advancingPlayers.AddRange(skippingPlayers);
                players.RemoveAll(p => advancingPlayers.Contains(p));
            }

            TournamentPlayer firstPlayer = null;
            var matches = new List<MatchSimulator>();

            foreach (var tournamentPlayer in players)
            {
                if (firstPlayer == null)
                    firstPlayer = tournamentPlayer;
                else // start new match
                {
                    SimulationManager.StartMatch(firstPlayer.Player.Name, firstPlayer.PlayerAi, 
                        tournamentPlayer.Player.Name, tournamentPlayer.PlayerAi);
                    // update player states
                    firstPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;
                    tournamentPlayer.Player.PlayerState = PlayerState.PlayingTournamentPlaying;

                    // add match to matches
                    matches.Add(SimulationManager.GetMatchSimulator(firstPlayer.Player.Name));
                }
            }

            await Task.WhenAll(matches.Select(m => m.CurrentSimulationTask).ToArray());

            players.ForEach(p => p.Player.PlayerState = PlayerState.PlayingTournamentWaiting);

            

            SavePlayers(players);
            // ...
            
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

        public void LeaveRunningTournament(Player player)
        {

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
