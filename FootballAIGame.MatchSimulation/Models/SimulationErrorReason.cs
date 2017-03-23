using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents a reason behind an error that happened during a match's simulation.
    /// </summary>
    [DataContract]
    public enum SimulationErrorReason
    {
        /// <summary>
        /// The speed of player is too high.
        /// </summary>
        [EnumMember]
        TooHighSpeed,

        /// <summary>
        /// The player's acceleration is too high.
        /// </summary>
        [EnumMember]
        TooHighAcceleration,

        /// <summary>
        /// The player's kick is too strong.
        /// </summary>
        [EnumMember]
        TooStrongKick,

        /// <summary>
        /// The player's movement vector is not set correctly (e.g. NaN value).
        /// </summary>
        [EnumMember]
        InvalidMovementVector,

        /// <summary>
        /// The player's kick vector is not set 
        /// </summary>
        [EnumMember]
        InvalidKickVector,

        /// <summary>
        /// The client has disconnected.
        /// </summary>
        [EnumMember]
        Disconnection,

        /// <summary>
        /// The match was canceled by the client.
        /// </summary>
        [EnumMember]
        Cancellation
    }
}
