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
    class Entry
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
                }
            }

            // set console exit handler
            Handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(Handler, true);

            // start listening
            var manager = ConnectionManager.Instance; ;
            manager.StartListening().Wait(); 
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                // console closing event
                var manager = ConnectionManager.Instance;
                using (var context = new ApplicationDbContext())
                {
                    var players = context.Players.ToList();

                    foreach (var player in players)
                    {
                        player.SelectedAi = null;
                        player.ActiveAis = null;
                        if (player.PlayerState == PlayerState.PlayingMatch)
                            player.PlayerState = PlayerState.Idle; // todo show browser clients that error has occured
                    }

                    context.SaveChanges();
                }
            }
            return false;
        }
        static ConsoleEventDelegate Handler;   // Keeps it from getting garbage collected

        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);


    }
}
