namespace FootballAIGame.MatchSimulation.Messages
{
    /// <summary>
    /// Represents a login message received from a client.
    /// </summary>
    /// <seealso cref="ClientMessage" />
    public class LoginMessage : ClientMessage
    {
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        /// <value>
        /// The name of the player.
        /// </value>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the name of the AI.
        /// </summary>
        /// <value>
        /// The name of the AI.
        /// </value>
        public string AiName { get; set; }
    }
}
