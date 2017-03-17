using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.LocalConsoleSimulator.CommandParsing;
using FootballAIGame.LocalConsoleSimulator.Commands;
using FootballAIGame.LocalSimulationBase;

namespace FootballAIGame.LocalConsoleSimulator
{
    static class Program
    {
        public static bool IsVerbose { get; private set; }

        private static void Main(string[] args)
        {
            IsVerbose = args.Contains("-v");

            Task.Run(() => SimulationManager.Instance.StartAcceptingConnections());

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
