
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class InvalidNumberOfAi : IParsingError
    {
        public string ErrorMessage { get; } = "The number of AI is not even.";
    }
}
