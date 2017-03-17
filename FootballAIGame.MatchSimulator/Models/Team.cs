using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
