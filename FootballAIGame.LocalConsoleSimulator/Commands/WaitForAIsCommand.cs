using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.LocalSimulationBase;

namespace FootballAIGame.LocalConsoleSimulator.Commands
{
    /// <summary>
    /// Represents the wait command.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.Commands.ICommand" />
    class WaitForAIsCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the names of the AIs for waiting.
        /// </summary>
        /// <value>
        /// The <see cref="List{T}"/> of the AIs' names.
        /// </value>
        public List<string> AINames { get; set; } = new List<string>();

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execute operation.
        /// </returns>
        public async Task ExecuteAsync()
        {
            foreach (var ai in AINames)
            {
                await SimulationManager.Instance.WaitForAIToConnectAsync(ai);
            }
        }
    }
}
