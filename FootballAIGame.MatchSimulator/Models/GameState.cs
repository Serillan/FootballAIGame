namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents the state of the football game that is sent
    /// to the AI applications from the server.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Gets or sets the football players array consisting of 22 players, where first 11
        /// players are from the first team and the rest 11 players are from the second team.
        /// </summary>
        /// <value>
        /// The football players array.
        /// </value>
        public FootballPlayer[] FootballPlayers { get; set; }

        /// <summary>
        /// Gets or sets the football ball.
        /// </summary>
        /// <value>
        /// The ball.
        /// </value>
        public Ball Ball { get; set; }

        /// <summary>
        /// Gets or sets the simulation step number from which this instance is.
        /// </summary>
        public int Step { get; set; }

        public bool KickOff { get; set; }
    }
}
