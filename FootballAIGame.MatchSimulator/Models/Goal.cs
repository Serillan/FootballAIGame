using System;

namespace FootballAIGame.MatchSimulation.Models
{
    public class Goal
    {
        public string ScoreTime { get; set; }

        public Team TeamThatScored { get; set; }

        public int ScorerNumber { get; set; }
    }
}
