
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
