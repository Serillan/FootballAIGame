using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class PathTooLong : IParsingError
    {
        public string ErrorMessage { get; }

        public string Path { get; }

        public PathTooLong(string path)
        {
            Path = path;
            ErrorMessage = $"The specified path is too long - {path}";
        }
    }
}
