using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.Commands
{
    interface ICommand
    {
        Task ExecuteAsync();
    }
}
