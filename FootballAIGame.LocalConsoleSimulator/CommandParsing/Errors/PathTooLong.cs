
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    /// <summary>
    /// Represents the path too long error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class PathTooLong : IParsingError
    {
        /// <summary>
        /// Gets the path that caused the error.
        /// </summary>
        /// <value>
        /// The invalid path.
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
        /// Initializes a new instance of the <see cref="PathTooLong"/> class.
        /// </summary>
        /// <param name="path">The path that is too long.</param>
        public PathTooLong(string path)
        {
            Path = path;
            ErrorMessage = $"The specified path is too long - {path}";
        }
    }
}
