using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    [DataContract]
    public class MatchInfo
    {
        [DataMember]
        public List<float> MatchData { get; set; }

        [DataMember]
        public List<Goal> Goals { get; set; }

        [DataMember]
        public List<SimulationError> Errors { get; set; }

        [DataMember]
        public Team? Winner { get; set; }

        [DataMember]
        public TeamStatistics Team1Statistics { get; set; }

        [DataMember]
        public TeamStatistics Team2Statistics { get; set; }

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
