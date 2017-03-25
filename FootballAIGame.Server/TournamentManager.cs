using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.Server.Models;

namespace FootballAIGame.Server
{
    public class TournamentManager
    {
        /// <summary>
        /// Gets or sets the running tournaments.
        /// </summary>
        /// <value>
        /// The running tournaments.
        /// </value>
        public List<TournamentSimulator> RunningTournaments { get; set; } = new List<TournamentSimulator>();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static TournamentManager Instance => _instance ?? (_instance = new TournamentManager());

        private static TournamentManager _instance; // singleton instance

        private TournamentManager() { }

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
        /// </summary>
        /// <param name="tournament">The recurring tournament.</param>
        /// <returns>The created tournament.</returns>
        public static void PlanNextRecurring(RecurringTournament tournament)
        {
            Tournament nextTournament;

            using (var context = new ApplicationDbContext())
            {
                var reccuringTournament = context.RecurringTournaments
                    .Include(t => t.Tournaments)
                    .SingleOrDefault(t => t.Id == tournament.Id);

                if (reccuringTournament == null)
                    return;

                var lastTournamentTime = reccuringTournament.Tournaments.Max(t => t.StartTime);
                nextTournament = new Tournament(reccuringTournament,
                    lastTournamentTime + TimeSpan.FromMinutes(reccuringTournament.RecurrenceInterval));
                context.Tournaments.Add(nextTournament);
                context.SaveChanges();
            }

            var simulator = new TournamentSimulator(nextTournament);
            simulator.PlanSimulation();
        }

        /// <summary>
        /// Player will leave a running tournament in which he currently is.
        /// If there is not such tournament, then it does nothing.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        public void LeaveRunningTournament(string playerName)
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
            }
        }

        /// <summary>
        /// Closes the running tournaments.
        /// </summary>
        /// <param name="context">The context.</param>
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
                        PlanNextRecurring(runningTournament.RecurringTournament);
                }
            }
        }
    }
}
