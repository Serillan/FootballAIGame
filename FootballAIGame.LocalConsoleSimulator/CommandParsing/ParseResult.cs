using FootballAIGame.LocalConsoleSimulator.CommandParsing.Errors;
using FootballAIGame.LocalConsoleSimulator.Commands;

namespace FootballAIGame.LocalConsoleSimulator.CommandParsing
{
    /// <summary>
    /// Represents the information about whether the parse was successful. If the parsing was not
    /// successful then it holds the <see cref="IParsingError"/> instance that describes the error that happened; 
    /// otherwise, it holds the parsed <see cref="ICommand"/>.
    /// </summary>
    class ParseResult
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parsing was successful.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parsing was successful; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the parsing error that caused the parsing to not be successful.
        /// </summary>
        /// <value>
        /// The parsing error.
        /// </value>
        public IParsingError Error { get; set; }
    }
}
