
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class EmptySavePath : IParsingError
    {
        public string ErrorMessage { get; } = "The save path is empty.";
    }
}
