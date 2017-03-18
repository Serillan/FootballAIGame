using System.Runtime.Serialization;

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
