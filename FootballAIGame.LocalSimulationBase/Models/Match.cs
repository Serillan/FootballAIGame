using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.LocalSimulationBase.Models
{
    public class Match
    {
        public MatchInfo MatchInfo { get; set; }

        public string Ai1Name { get; set; }

        public string Ai2Name { get; set; }
    }
}
