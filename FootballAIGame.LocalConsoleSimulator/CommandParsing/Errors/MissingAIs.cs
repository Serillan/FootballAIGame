
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class MissingAIs : IParsingError
    {
        public string ErrorMessage { get; } = "There are no AIs specified.";
    }
}
