
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class InvalidPath : IParsingError
    {
        public string ErrorMessage { get; }

        public string Path { get; }

        public InvalidPath(string path)
        {
            Path = path;
            ErrorMessage = $"The specified path is not valid - {path}";
        }
    }
}
