
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class MissingCommand : IParsingError
    {
        public string ErrorMessage { get; } = "Missing command.";
    }
}
