using System;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.LocalConsoleSimulator.CommandParsing;
using FootballAIGame.LocalSimulationBase;
using FootballAIGame.MatchSimulation;

namespace FootballAIGame.LocalConsoleSimulator
{
    static class Program
    {
        public static bool IsVerbose { get; private set; }

        private static void Main(string[] args)
        {
            if (args.Contains("-v"))
            {
                IsVerbose = true;
                ConnectionManager.Instance.IsVerbose = true;
            }

            Task.Run(() => SimulationManager.Instance.StartAcceptingConnectionsAsync());

            if (IsVerbose)
                Console.WriteLine("Listening has started.");

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
