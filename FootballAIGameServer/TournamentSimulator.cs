using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        private TimeSpan TimeUntilStart => StartTime - DateTime.Now;

        public static List<TournamentSimulator> RunningTournaments { get; set; }

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
            if (TimeUntilStart.TotalSeconds > 0)
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
            // get tournament

            // set state -> check if there is enough people

            // update players

            // while there is more than 1 player -> round simulation

            // set state to finished
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

    }
}
