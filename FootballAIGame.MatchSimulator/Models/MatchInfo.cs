using System.Collections.Generic;

namespace FootballAIGame.MatchSimulation.Models
{
    public class MatchInfo
    {
        public List<float> MatchData { get; set; }

        public List<Goal> Goals { get; set; }

        public List<SimulationError> Errors { get; set; }

        public Team? Winner { get; set; }

        public TeamStatistics Team1Statistics { get; set; }

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
