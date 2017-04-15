using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents the football statistics of a football team in a football match.
    /// </summary>
    [DataContract]
    public class TeamStatistics
    {
        /// <summary>
        /// Gets or sets the number of goals scored by the team.
        /// </summary>
        /// <value>
        /// The number of goals scored by the team.
        /// </value>
        [DataMember]
        public int Goals { get; set; }

        /// <summary>
        /// Gets or sets the number of the team's shots.
        /// </summary>
        /// <value>
        /// The number of the team's shots.
        /// </value>
        [DataMember]
        public int Shots { get; set; }

        /// <summary>
        /// Gets or sets the number of the team's shots on target.
        /// </summary>
        /// <value>
        /// The number of the team's shots on target.
        /// </value>
        [DataMember]
        public int ShotsOnTarget { get; set; }

        /// <summary>
        /// Gets or sets the average time that the simulator waited for the team's action to be received.
        /// </summary>
        /// <value>
        /// The average time that the simulator waited for the team's action to be received.
        /// </value>
        public int AverageActionLatency { get; set; }
    }
}
