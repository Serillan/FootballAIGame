using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.Commands
{
    interface ICommand
    {
        Task ExecuteAsync();
    }
}
