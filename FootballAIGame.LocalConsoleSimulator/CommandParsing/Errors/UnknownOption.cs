namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    /// <summary>
    /// Represents the unknown option error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class UnknownOption : IParsingError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the unknown option.
        /// </summary>
        /// <value>
        /// The unknown option.
        /// </value>
        public string Option { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownOption"/> class.
        /// </summary>
        /// <param name="option">The unknown option.</param>
        public UnknownOption(string option)
        {
            Option = option;
            ErrorMessage = $"Unknown option - {option}";
        }
    }
}
