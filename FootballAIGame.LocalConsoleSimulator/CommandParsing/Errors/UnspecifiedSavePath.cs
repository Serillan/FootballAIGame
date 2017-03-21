
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    /// <summary>
    /// Represents the unspecified save path error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class UnspecifiedSavePath : IParsingError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; } = "The save path is not specified.";
    }
}
