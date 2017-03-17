using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class UnspecifiedSavePath : IParsingError
    {
        public string ErrorMessage { get; } = "The save path is not specified.";
    }
}
