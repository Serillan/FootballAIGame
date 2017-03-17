using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class MissingOptions : IParsingError
    {
        public string ErrorMessage { get; } = "Command options are missing.";
    }
}
