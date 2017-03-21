
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{

    /// <summary>
    /// Represents the invalid command name parsing error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class InvalidCommandName : IParsingError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandName"/> class.
        /// </summary>
        /// <param name="name">The invalid command name.</param>
        public InvalidCommandName(string name)
        {
            ErrorMessage = $"{name} is not a valid command name.";
        }
    }
}
