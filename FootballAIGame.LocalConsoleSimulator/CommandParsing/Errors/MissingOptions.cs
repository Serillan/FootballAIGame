
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class MissingOptions : IParsingError
    {
        public string ErrorMessage { get; } = "Command options are missing.";
    }
}
