
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{

    /// <summary>
    /// Represents the empty save path parsing error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class EmptySavePath : IParsingError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; } = "The save path is empty.";
    }
}
