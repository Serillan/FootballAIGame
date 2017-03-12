namespace FootballAIGame.MatchSimulation.Models
{
    public class TeamStatistics
    {
        /// <summary>
        /// Gets or sets the number of team goals.
        /// </summary>
        /// <value>
        /// The number of player1 team shots.
        /// </value>
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of team shots.
        /// </summary>
        /// <value>
        /// The number of player1 team shots.
        /// </value>
        public int Shots { get; set; }

        /// <summary>
        /// Gets or sets the number of team shots on target.
        /// </summary>
        /// <value>
        /// The number of player1 team shots on target.
        /// </value>
        public int ShotsOnTarget { get; set; }
    }
}
