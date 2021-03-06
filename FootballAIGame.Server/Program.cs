﻿using System;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation;
using FootballAIGame.Server.ApiForWeb;
using FootballAIGame.DbModel.Models;

namespace FootballAIGame.Server
{
    /// <summary>
    /// Provides entry point of the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The default listening port. This port is used for listening if the user
        /// doesn't provide his own port in the program's argument.
        /// </summary>
        private const int DefaultListeningPort = 50030;

        /// <summary>
        /// The console event handler.
        /// We use this field to protect the handler from getting garbage collected.
        /// </summary>
        private static ConsoleEventDelegate _handler;

        /// <summary>
        /// The entry point of the application. Starts the <see cref="GameServerService" /> and
        /// start listening for new AI connections.
        /// <para /> Also sets application closing handler.
        /// </summary>
        /// <param name="args">A list of command line arguments.</param>
        public static void Main(string[] args)
        {
            int port = DefaultListeningPort;

            // the port can be specified in the first program's argument (options are ignored)
            foreach (var arg in args)
            {
                if (!arg.StartsWith("-"))
                {
                    if (!int.TryParse(arg, out port))
                    {
                        Console.Error.WriteLine($"Invalid specified port: {arg}");
                        return;
                    }
                    
                    break;
                }
            }

            try
            {
                // start listening
                var listeningTask = SimulationManager.Instance.StartAcceptingConnectionsAsync(port);

                if (listeningTask.IsCompleted) // used address
                    return;

                // start tournaments
                TournamentManager.PlanUnstartedTournaments();

                // start services
                var host = new ServiceHost(typeof(GameServerService));
                host.Open();
                Console.WriteLine("Services have started.");

                // set console exit handler and put it to _handler so that it won't get garbage collected
                _handler = ConsoleEventHandler;
                SetConsoleCtrlHandler(_handler, true);

                // start regular checks (every 10s) of the database
                var databaseChecks = CheckDatabaseStateRegularlyAsync(10000);

                // stay on while the database is online and the listening is active
                Task.WhenAny(listeningTask, databaseChecks).Wait();
            }
            catch (Exception ex) when (ex is SqlException || ex is EntityException)
            {
                Console.Error.WriteLine("Error: Cannot connect to the database.");
            }
            catch (AddressAlreadyInUseException) // WCF service address used
            {
                // either there is already another server on
                // or there is another process using the GameServerService's address
                Console.Error.WriteLine("Error: WCF service's port is already being used.\n" +
                                        "You can change the port in FootballAIGame.Server.exe.config but" +
                                        "in that case you need to change the port in the web application as " +
                                        "well (in Web.config file).");
            }
        }

        /// <summary>
        /// Callback that is called every time the console event is raised.
        /// It is used for handling a closing of the application.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        private static bool ConsoleEventHandler(int eventType)
        {
            if (eventType != 2 && eventType != 4) return false; // if application is not being closed

            using (var context = new ApplicationDbContext())
            {
                var players = context.Players.ToList();
                var challenges = context.Challenges.ToList();

                foreach (var player in players)
                {
                    player.SelectedAI = null;
                    player.ActiveAIs = null;
                    player.PlayerState = PlayerState.Idle;
                }

                context.Challenges.RemoveRange(challenges);

                TournamentManager.Instance.CloseRunningTournaments(context);

                context.SaveChanges();
            }
            return false;
            // console closing event
        }

        /// <summary>
        /// Regularly checks the database state asynchronously.
        /// Returns when the database isn't online anymore.
        /// </summary>
        /// <param name="checkInterval">The checks' interval in milliseconds.</param>
        /// <returns>
        /// The task that represents the asynchronous regular checks.
        /// The task is completed when the database goes offline.
        /// </returns>
        private static async Task CheckDatabaseStateRegularlyAsync(int checkInterval)
        {
            var context = new ApplicationDbContext();

            while (true)
            {
                if (!context.Database.Exists())
                {
                    Console.Error.WriteLine("Error: Cannot connect to the database.");
                    return;
                }

                await Task.Delay(checkInterval);
            }
        }

        /// <summary>
        /// Adds or removes an application-defined HandlerRoutine function from the list of handler functions for the calling process.
        /// </summary>
        /// <param name="handlerRoutine">A delegate to the application-defined HandlerRoutine function to be added or removed.</param>
        /// <param name="add">If this parameter is <c>true</c>, the handler is added; if it is <c>false</c>, the handler is removed.</param>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate handlerRoutine, bool add);

        /// <summary>
        /// Console event handler delegate.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>If it returns <c>false</c>, 
        /// the next handler function in the list of handlers for this process is used.</returns>
        private delegate bool ConsoleEventDelegate(int eventType);
    }
}
