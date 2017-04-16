using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents the football's goal.
    /// </summary>
    [DataContract]
    public class Goal
    {
        /// <summary>
        /// Gets or sets the time in which the goal was scored.
        /// </summary>
        /// <value>
        /// The score time in the following format: "minutes:seconds".
        /// </value>
        [DataMember]
        public string ScoreTime { get; set; }

        /// <summary>
        /// Gets or sets the team that scored the goal.
        /// </summary>
        /// <value>
        /// The team that scored the goal.
        /// </value>
        [DataMember]
        public Team TeamThatScored { get; set; }

        /// <summary>
        /// Gets or sets the number of team's player who scored the goal.
        /// </summary>
        /// <value>
        /// The number of the scorer.
        /// </value>
        [DataMember]
        public int ScorerNumber { get; set; }
    }
}
