using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.Commands
{
    /// <summary>
    /// Represents the command.
    /// Defines method to execute the command code asynchronously.
    /// </summary>
    interface ICommand
    {
        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous execute operation.</returns>
        Task ExecuteAsync();
    }
}
