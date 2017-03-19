using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    [DataContract]
    public class TeamStatistics
    {
        /// <summary>
        /// Gets or sets the number of team goals.
        /// </summary>
        /// <value>
        /// The number of player1 team shots.
        /// </value>
        [DataMember]
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of team shots.
        /// </summary>
        /// <value>
        /// The number of player1 team shots.
        /// </value>
        [DataMember]
        public int Shots { get; set; }

        /// <summary>
        /// Gets or sets the number of team shots on target.
        /// </summary>
        /// <value>
        /// The number of player1 team shots on target.
        /// </value>
        [DataMember]
        public int ShotsOnTarget { get; set; }
    }
}
