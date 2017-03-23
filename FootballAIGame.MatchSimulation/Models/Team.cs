using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Identifies the football team in the match.
    /// </summary>
    [DataContract]
    public enum Team
    {
        /// <summary>
        /// The first player's team.
        /// </summary>
        [EnumMember]
        FirstPlayer,

        /// <summary>
        /// The second player's team.
        /// </summary>
        [EnumMember]
        SecondPlayer

    }
}
