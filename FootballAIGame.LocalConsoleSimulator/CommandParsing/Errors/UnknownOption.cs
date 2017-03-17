namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    class UnknownOption : IParsingError
    {
        public string ErrorMessage { get; }

        public string Option { get; }

        public UnknownOption(string option)
        {
            Option = option;
            ErrorMessage = $"Unknown option - {option}";
        }
    }
}
