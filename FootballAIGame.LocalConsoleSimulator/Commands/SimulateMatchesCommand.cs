using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.LocalSimulationBase;

namespace FootballAIGame.LocalConsoleSimulator.Commands
{
    class SimulateMatchesCommand : ICommand
    {
        public bool ExtendedResultOn { get; set; }

        public bool SavingOn { get; set; }

        public DirectoryInfo SavingDirectory { get; set; }

        public FileInfo[] SavingFiles { get; set; }

        public List<Tuple<string, string>> Opponents { get; set; }

        public async Task ExecuteAsync()
        {





        }
    }
}
