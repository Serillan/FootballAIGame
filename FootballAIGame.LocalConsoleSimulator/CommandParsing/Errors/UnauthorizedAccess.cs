
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    /// <summary>
    /// Represents the unauthorized access error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class UnauthorizedAccess : IParsingError
    {
        /// <summary>
        /// Gets the path that caused the error.
        /// </summary>
        /// <value>
        /// The path that caused the error.
        /// </value>
        public string Path { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedAccess"/> class.
        /// </summary>
        /// <param name="path">The path that caused the error.</param>
        public UnauthorizedAccess(string path)
        {
            Path = path;
            ErrorMessage = $"Unauthorized access - {path}";
        }
    }
}
