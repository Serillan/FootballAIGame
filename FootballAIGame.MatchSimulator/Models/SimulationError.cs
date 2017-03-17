using System;
using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    [DataContract]
    public class SimulationError
    {
        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public SimulationErrorReason Reason { get; set; }

        [DataMember]
        public Team Team { get; set; }

        [DataMember]
        public int? AffectedPlayerNumber { get; set; }

    }
}
