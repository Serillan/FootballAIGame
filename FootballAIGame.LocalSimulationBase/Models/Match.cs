using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootballAIGame.LocalSimulationBase.Models
{
    
    [DataContract]
    public class Match
    {
        [DataMember]
        public string Ai1Name { get; set; }

        [DataMember]
        public string Ai2Name { get; set; }

        [DataMember]
        public string Winner { get; set; }

        [DataMember]
        public List<Goal> Goals { get; set; }

        [DataMember]
        public List<string> Ai1Errors { get; set; }

        [DataMember]
        public List<string> Ai2Errors { get; set; }

        [DataMember]
        public List<float> MatchData { get; set; }

        /// <summary>
        /// Gets or sets the number of player1 team shots.
        /// </summary>
        /// <value>
        /// The number of player1 team shots.
        /// </value>
        public int Shots1 { get; set; }

        /// <summary>
        /// Gets or sets the number of player2 team shots.
        /// </summary>
        /// <value>
        /// The number of player2 team shots.
        /// </value>
        public int Shots2 { get; set; }

        /// <summary>
        /// Gets or sets the number of player1 team shots on target.
        /// </summary>
        /// <value>
        /// The number of player1 team shots on target.
        /// </value>
        public int ShotsOnTarget1 { get; set; }

        /// <summary>
        /// Gets or sets the number of player2 team shots on target.
        /// </summary>
        /// <value>
        /// The number of player2 team shots on target.
        /// </value>
        public int ShotsOnTarget2 { get; set; }

    }
}
