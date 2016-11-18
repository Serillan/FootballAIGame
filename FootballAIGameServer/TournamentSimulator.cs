using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.Models;

namespace FootballAIGameServer
{
    public class TournamentSimulator
    {

        private ConnectionManager ConnectionManager { get; set; }
        private int TournamentId { get; set; }
        private DateTime StartTime { get; set; }

        private TimeSpan TimeUntilStart => DateTime.Now - StartTime;

        public static List<TournamentSimulator> RunningTournaments { get; set; }

        public static void PlanNextTournaments()
        {
            using (var context = new ApplicationDbContext())
            {
                var nextTournaments = context.Tournaments.ToList();

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
        }

        public async Task PlanSimulation()
        {
            Console.WriteLine($"Simulation {TournamentId} is being planned!");

            
            // sleep until 5 minutes before tournament
            if (TimeUntilStart.Minutes > 5)
            {
                await Task.Delay(TimeUntilStart - TimeSpan.FromMinutes(5));
                Console.WriteLine($"Simulation {TournamentId} is awaken 5 minutes before start!");
                KickInactive();
            }

            // when tournament starts
            if (TimeUntilStart.Seconds > 0)
                await Task.Delay(TimeUntilStart);

            Console.WriteLine($"Simulation {TournamentId} is being simulated!");
            KickInactive();
            await Simulate();
        }

        public void LeaveRunningTournament(Player player)
        {

        }

        private async Task Simulate()
        {
            
        }

        private void KickInactive()
        {
            
        }

    }
}
