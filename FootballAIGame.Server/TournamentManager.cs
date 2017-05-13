using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Server
{
    /// <summary>
    /// Provides the functionality to manage tournament simulations. Implemented as singleton.
    /// </summary>
    class TournamentManager
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static TournamentManager _instance;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static TournamentManager Instance => _instance ?? (_instance = new TournamentManager());

        /// <summary>
        /// Gets or sets the running tournaments.
        /// </summary>
        /// <value>
        /// The list of running tournaments.
        /// </value>
        public List<TournamentSimulator> RunningTournaments { get; set; } = new List<TournamentSimulator>();

        /// <summary>
        /// Prevents a default instance of the <see cref="TournamentManager"/> class from being created.
        /// </summary>
        private TournamentManager() { }

        /// <summary>
        /// Plans the specified tournament.
        /// </summary>
        /// <param name="tournamentId">The tournament's ID.</param>
        public static void PlanTournament(int tournamentId)
        {
            using (var context = new ApplicationDbContext())
            {
                var tournament = context.Tournaments.SingleOrDefault(t => t.Id == tournamentId);
                if (tournament == null)
                    return;

                var simulator = new TournamentSimulator(tournament);
                simulator.PlanSimulation();
            }
        }

        /// <summary>
        /// Creates and plans the next tournament from the specified <see cref="RecurringTournament"/>.
        /// If a recurring tournament with the specified ID doesn't exist, does nothing.
        /// </summary>
        /// <param name="id">The recurring tournament's ID.</param>
        public static void PlanNextRecurring(int id)
        {
            Tournament nextTournament;

            using (var context = new ApplicationDbContext())
            {
                var reccuringTournament = context.RecurringTournaments
                    .Include(t => t.Tournaments)
                    .SingleOrDefault(t => t.Id == id);

                if (reccuringTournament == null)
                    return;

                // the current time if there are no tournaments
                var lastTournamentTime = DateTime.Now; 
                if (reccuringTournament.Tournaments.Any(t => true))
                    lastTournamentTime = reccuringTournament.Tournaments.Max(t => t.StartTime);

                nextTournament = new Tournament()
                {
                    StartTime = lastTournamentTime + TimeSpan.FromMinutes(reccuringTournament.RecurrenceInterval),
                    TournamentState = TournamentState.Unstarted,
                    Name = reccuringTournament.Name,
                    MinimumNumberOfPlayers = reccuringTournament.MinimumNumberOfPlayers,
                    MaximumNumberOfPlayers = reccuringTournament.MaximumNumberOfPlayers,
                    RecurringTournament = reccuringTournament
                };

                context.Tournaments.Add(nextTournament);
                context.SaveChanges();
            }

            var simulator = new TournamentSimulator(nextTournament);
            simulator.PlanSimulation();
        }

        /// <summary>
        /// Plans the unstarted tournaments.
        /// </summary>
        public static void PlanUnstartedTournaments()
        {
            using (var context = new ApplicationDbContext())
            {
                var nextTournaments = context.Tournaments
                    .Where(t => t.TournamentState == TournamentState.Unstarted)
                    .ToList();

                foreach (var nextTournament in nextTournaments)
                {
                    var simulator = new TournamentSimulator(nextTournament);
                    simulator.PlanSimulation();
                }

                Console.WriteLine("Tournaments planned.");
            }
        }

        /// <summary>
        /// Removes the specified player from a running tournament in which he currently is.
        /// If there is not such tournament, then it does nothing.
        /// </summary>
        /// <param name="playerName">The player's name.</param>
        public void RemoveFromRunningTournament(string playerName)
        {
            TournamentSimulator tournamentSimulator;

            lock (RunningTournaments)
            {
                tournamentSimulator = RunningTournaments
                    .SingleOrDefault(t => t.Players.Any(p => p.Player.Name == playerName));
            }

            if (tournamentSimulator == null)
                return;

            lock (tournamentSimulator.Players)
            {
                tournamentSimulator.Players.RemoveAll(p => p.Player.Name == playerName);
            }

            // improve position for players that have already ended
            using (var context = new ApplicationDbContext())
            {
                var playersWithPosition = context.TournamentPlayers
                    .Where(tp => tp.TournamentId == tournamentSimulator.TournamentId && tp.PlayerPosition != null);

                foreach (var tournamentPlayer in playersWithPosition)
                {
                    tournamentPlayer.PlayerPosition--;
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Closes running tournaments.
        /// </summary>
        /// <param name="context">The db context.</param>
        public void CloseRunningTournaments(ApplicationDbContext context)
        {
            lock (RunningTournaments)
            {
                var runningTournaments = context.Tournaments
                    .Include(t => t.RecurringTournament)
                    .Where(t => t.TournamentState == TournamentState.Running);

                foreach (var runningTournament in runningTournaments)
                {
                    runningTournament.TournamentState = TournamentState.ErrorClosed;
                    if (runningTournament.RecurringTournament != null)
                        PlanNextRecurring(runningTournament.RecurringTournament.Id);
                }
            }
        }
    }
}
