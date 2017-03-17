using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class InvalidCommandName : IParsingError
    {
        public string ErrorMessage { get; }

        public InvalidCommandName(string name)
        {
            ErrorMessage = $"{name} is not a valid command name.";
        }
    }
}
