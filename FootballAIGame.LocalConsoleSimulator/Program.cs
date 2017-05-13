using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FootballAIGame.LocalConsoleSimulator.CommandParsing;
using FootballAIGame.LocalSimulationBase;
using FootballAIGame.MatchSimulation;

namespace FootballAIGame.LocalConsoleSimulator
{
    /// <summary>
    /// Provides entry point of the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Gets a value indicating whether this application is in the verbose mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if the application is in the verbose mode; otherwise, <c>false</c>.
        /// </value>
        public static bool IsVerbose { get; private set; }

        /// <summary>
        /// The entry point of the application. Starts the listening for AI connections. Starts processing
        /// the input commands.
        /// </summary>
        /// <param name="args"> A list of command line arguments. If it contains '-v' then it starts
        /// the application in the verbose mode.</param>
        private static void Main(string[] args)
        {
            if (args.Contains("-v"))
            {
                IsVerbose = true;
                ConnectionManager.Instance.IsVerbose = true;
            }

            // start listening
            var listeningTask = SimulationManager.Instance.StartAcceptingConnectionsAsync();

            if (listeningTask.IsCompleted) // used address
                return;

            string line;

            while ((line = Console.ReadLine()) != null)
            {
                var parseResult = CommandParser.TryParse(line);

                if (parseResult.IsSuccessful)
                    parseResult.Command.ExecuteAsync().Wait();
                else
                    Console.Error.WriteLine($"Error : {parseResult.Error.ErrorMessage}");
            }

        }

    }
}
