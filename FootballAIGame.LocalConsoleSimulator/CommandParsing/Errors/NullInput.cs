using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class NullInput : IParsingError
    {
        public string ErrorMessage { get; } = "The input is null";
    }
}
