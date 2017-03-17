using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class EmptySavePath : IParsingError
    {
        public string ErrorMessage { get; } = "The save path is empty.";
    }
}
