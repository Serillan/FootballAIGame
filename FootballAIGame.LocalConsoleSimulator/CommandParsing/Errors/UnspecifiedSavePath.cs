
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class UnspecifiedSavePath : IParsingError
    {
        public string ErrorMessage { get; } = "The save path is not specified.";
    }
}
