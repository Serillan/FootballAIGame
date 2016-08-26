using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.ApiForWeb;
using FootballAIGameServer.Models;

namespace FootballAIGameServer
{
    public class Entry
    {
        public static void Main(string[] args)
        {
            // start services
            var host = new ServiceHost(typeof(GameServerService));
            host.Open(); 
            Console.WriteLine("Services have started.");

            // reset player states on the start
            using (var context = new ApplicationDbContext())
            {
                var players = context.Players.ToList();
                foreach (var player in players)
                {
                    player.PlayerState = PlayerState.Idle;
                    player.ActiveAis = null;
                    player.SelectedAi = null;
                }
                context.SaveChanges();
            }

            // set console exit handler
            Handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(Handler, true);

            // initialize random
            MatchSimulator.Random = new Random();

            // start listening
            var manager = ConnectionManager.Instance;
            manager.StartListening().Wait(); 
        }

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2 || eventType == 4)
            {
                var manager = ConnectionManager.Instance;
                using (var context = new ApplicationDbContext())
                {
                    var players = context.Players.ToList();

                    foreach (var player in players)
                    {
                        player.SelectedAi = null;
                        player.ActiveAis = null;
                        player.PlayerState = PlayerState.Idle; // todo show browser clients that error has occured
                    }

                    context.SaveChanges();
                }
                return false;
            }
            // console closing event
            return false;
        }

        static ConsoleEventDelegate Handler;  // Keeps it from getting garbage collected

        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);


    }
}
