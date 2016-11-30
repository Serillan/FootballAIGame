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
    /// <summary>
    /// The class that contains the entry point of the application.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// The entry point of the application. Starts the <see cref="GameServerService"/> and
        /// start listening on port <see cref="ConnectionManager.GameServerPort"/> for new AI
        /// connections. <para /> Also sets application closing handler.
        /// </summary>
        public static void Main()
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
            _handler = ConsoleEventHandler;
            SetConsoleCtrlHandler(_handler, true);

            // initialize random
            MatchSimulator.Random = new Random();

            // plan tournaments
            TournamentSimulator.PlanUnstartedTournaments();
            Console.WriteLine("Tournaments planned.");

            // start listening
            var manager = ConnectionManager.Instance;
            manager.StartListening().Wait();

        }

        /// <summary>
        /// Callback that is called every time the console event is raised.
        /// It is used for handling closing of the application.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns></returns>
        private static bool ConsoleEventHandler(int eventType)
        {
            if (eventType != 2 && eventType != 4) return false; // if application is not being closed

            var manager = ConnectionManager.Instance;
            using (var context = new ApplicationDbContext())
            {
                var players = context.Players.ToList();

                foreach (var player in players)
                {
                    player.SelectedAi = null;
                    player.ActiveAis = null;
                    player.PlayerState = PlayerState.Idle; // TODO show browser clients that error has occurred
                }

                TournamentSimulator.CloseRunningTournaments(context);
                
                context.SaveChanges();
            }
            return false;
            // console closing event
        }

        /// <summary>
        /// Keeps handler from getting garbage collected.
        /// </summary>
        private static ConsoleEventDelegate _handler;

        /// <summary>
        /// Console event handler delegate.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>If it returns <c>false</c>, 
        /// the next handler function in the list of handlers for this process is used.</returns>
        private delegate bool ConsoleEventDelegate(int eventType);

        /// <summary>
        /// Adds or removes an application-defined HandlerRoutine function from the list of handler functions for the calling process.
        /// </summary>
        /// <param name="handlerRoutine">A delegate to the application-defined HandlerRoutine function to be added or removed.</param>
        /// <param name="add">If this parameter is <c>true</c>, the handler is added; if it is <c>false</c>, the handler is removed.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate handlerRoutine, bool add);


    }
}
