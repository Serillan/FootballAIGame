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
        public ErrorType Type { get; set; }

        [DataMember]
        public Team Team { get; set; }

        [DataMember]
        public int? AffectedPlayerNumber { get; set; }

        [DataContract]
        public enum ErrorType
        {
            [EnumMember]
            TooHighSpeed,

            [EnumMember]
            TooHighAcceleration,

            [EnumMember]
            TooStrongKick,

            [EnumMember]
            InvalidMovementVector,

            [EnumMember]
            InvalidKickVector,

            [EnumMember]
            Disconnection, Cancel
        }
    }
}
