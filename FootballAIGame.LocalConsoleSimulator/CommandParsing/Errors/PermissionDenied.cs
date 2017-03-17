using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class PermissionDenied : IParsingError
    {
        public string ErrorMessage { get; }

        public string Path { get; }

        public PermissionDenied(string path)
        {
            Path = path;
            ErrorMessage = $"Missing permission to use - {path}";
        }
    }
}
