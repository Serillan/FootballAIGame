namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents the state of the football game.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Gets or sets the football players array that should consist of 22 players, where first 11
        /// players are from the first team and other 11 players are from the second team.
        /// </summary>
        /// <value>
        /// The array of <see cref="FootballPlayer"/>s.
        /// </value>
        public FootballPlayer[] FootballPlayers { get; set; }

        /// <summary>
        /// Gets or sets the football ball.
        /// </summary>
        /// <value>
        /// The <see cref="Ball"/>.
        /// </value>
        public Ball Ball { get; set; }

        /// <summary>
        /// Gets or sets the simulation step to which this instance belongs.
        /// </summary>
        /// <value>
        /// The step number to which this instance belongs.
        /// </value>
        public int Step { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a kickoff is happening.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a kickoff is happening; otherwise, <c>false</c>.
        /// </value>
        public bool IsKickOff { get; set; }
    }
}
