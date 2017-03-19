using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    [DataContract]
    public enum Team
    {
        [EnumMember]
        FirstPlayer,

        [EnumMember]
        SecondPlayer

    }
}
