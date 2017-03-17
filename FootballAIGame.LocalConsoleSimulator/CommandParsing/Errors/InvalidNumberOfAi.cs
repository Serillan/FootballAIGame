using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class InvalidNumberOfAi : IParsingError
    {
        public string ErrorMessage { get; } = "The number of AI is not even.";
    }
}
