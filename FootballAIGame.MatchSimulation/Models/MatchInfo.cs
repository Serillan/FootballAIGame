using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Contains all the information about the match simulation.
    /// </summary>
    [DataContract]
    public class MatchInfo
    {
        /// <summary>
        /// Gets or sets the match data. For each step there should be a ball position
        /// and 22 players positions, where first 11 players are from the first team
        /// and other 11 players are from the second. <para />
        /// </summary>
        /// <value>
        /// If set correctly then there should be the match data in the following order: 
        /// step0BallX, step0BallY, step0Player0X, step0Player0Y, ...step0Player22Y, step1BallX ... 
        /// </value>
        [DataMember]
        public List<float> MatchData { get; set; }

        /// <summary>
        /// Gets or sets the goals that were scored during the match.
        /// </summary>
        /// <value>
        /// The <see cref="IList{T}"/> of <see cref="Goal"/> scored in the match.
        /// </value>
        [DataMember]
        public IList<Goal> Goals { get; set; }

        /// <summary>
        /// Gets or sets the errors that happened during the match simulation.
        /// </summary>
        /// <value>
        /// The <see cref="IList{T}"/> of <see cref="SimulationError"/> that happened during the match simulation.
        /// </value>
        [DataMember]
        public IList<SimulationError> Errors { get; set; }

        /// <summary>
        /// Gets or sets the team that won the match.
        /// </summary>
        /// <value>
        /// The team that won the match or null in case of draw.
        /// </value>
        [DataMember]
        public Team? Winner { get; set; }

        /// <summary>
        /// Gets or sets the second team's <see cref="TeamStatistics"/>.
        /// </summary>
        /// <value>
        /// The second team's <see cref="TeamStatistics"/>.
        /// </value>
        [DataMember]
        public TeamStatistics Team1Statistics { get; set; }

        /// <summary>
        /// Gets or sets the second team's <see cref="TeamStatistics"/>.
        /// </summary>
        /// <value>
        /// The second team's <see cref="TeamStatistics"/>.
        /// </value>
        [DataMember]
        public TeamStatistics Team2Statistics { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchInfo"/> class.
        /// </summary>
        public MatchInfo()
        {
            MatchData = new List<float>();
            Goals = new List<Goal>();
            Errors = new List<SimulationError>();
            Team1Statistics = new TeamStatistics();
            Team2Statistics = new TeamStatistics();
        }

    }

    
}
