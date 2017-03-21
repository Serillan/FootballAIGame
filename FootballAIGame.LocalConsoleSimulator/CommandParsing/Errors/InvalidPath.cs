
namespace FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors
{
    /// <summary>
    /// Represents the invalid path to some file or directory error.
    /// </summary>
    /// <seealso cref="FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors.IParsingError" />
    class InvalidPath : IParsingError
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the invalid path.
        /// </summary>
        /// <value>
        /// The invalid path.
        /// </value>
        public string Path { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPath"/> class.
        /// </summary>
        /// <param name="path">The invalid path.</param>
        public InvalidPath(string path)
        {
            Path = path;
            ErrorMessage = $"The specified path is not valid - {path}";
        }
    }
}
