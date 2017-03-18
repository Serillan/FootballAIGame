
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class NullInput : IParsingError
    {
        public string ErrorMessage { get; } = "The input is null";
    }
}
