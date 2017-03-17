using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors;
using FootballAIGame.LocalConsoleSimulator.Commands;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing
{
    class ParseResult
    {
        public ICommand Command { get; set; }

        public bool IsSuccessful { get; set; }

        public IParsingError Error { get; set; }
    }
}
