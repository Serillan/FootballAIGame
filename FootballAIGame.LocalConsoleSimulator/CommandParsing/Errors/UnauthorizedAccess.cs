using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class UnauthorizedAccess : IParsingError
    {
        public string ErrorMessage { get; }

        public string Path { get; }

        public UnauthorizedAccess(string path)
        {
            Path = path;
            ErrorMessage = $"Unauthorized access - {path}";
        }
    }
}
