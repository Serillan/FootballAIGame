
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    /// <summary>
    /// Represents the invalid number of AI parsing error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class InvalidNumberOfAi : IParsingError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; } = "The number of AI is not even.";
    }
}
