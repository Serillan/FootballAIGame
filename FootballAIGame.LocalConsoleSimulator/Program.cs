using System;
using System.IO;
using System.Threading.Tasks;
using FootballAIGame.LocalConsoleSimulator.CommandParsing;
using FootballAIGame.LocalConsoleSimulator.Commands;
using FootballAIGame.LocalSimulationBase;

namespace FootballAIGame.LocalConsoleSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => SimulationManager.Instance.StartAcceptingConnections());

            string line;

            while ((line = Console.ReadLine()) != null)
            {
                var parseResult = CommandParser.TryParse(line);

                if (parseResult.IsSuccessfull)
                    parseResult.Command.ExecuteAsync().Wait();
                else
                    Console.Error.WriteLine($"Parse error : {parseResult.Error.ErrorMessage}");
            }

        }

    }
}
