using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.MatchSimulation.Models
{
    [DataContract]
    public enum SimulationErrorReason
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
        Disconnection,

        [EnumMember]
        Cancellation
    }
}
