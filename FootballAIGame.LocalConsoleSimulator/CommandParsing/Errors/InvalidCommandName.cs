
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class InvalidCommandName : IParsingError
    {
        public string ErrorMessage { get; }

        public InvalidCommandName(string name)
        {
            ErrorMessage = $"{name} is not a valid command name.";
        }
    }
}
