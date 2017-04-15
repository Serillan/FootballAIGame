using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.LocalSimulationBase;

namespace FootballAIGame.LocalConsoleSimulator.Commands
{
    class WaitForAIsCommand : ICommand
    {
        public IList<string> AINames { get; set; } = new List<string>();

        public async Task ExecuteAsync()
        {
            foreach (var ai in AINames)
            {
                await SimulationManager.Instance.WaitForAIToConnectAsync(ai);
            }
        }
    }
}
