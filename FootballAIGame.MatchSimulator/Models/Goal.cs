using System;
using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    [DataContract]
    public class Goal
    {
        [DataMember]
        public string ScoreTime { get; set; }

        [DataMember]
        public Team TeamThatScored { get; set; }

        [DataMember]
        public int ScorerNumber { get; set; }
    }
}
