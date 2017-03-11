using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootballAIGame.LocalSimulationBase.Models
{
    
    [DataContract]
    public class Match
    {
        [DataMember]
        public string Ai1Name { get; set; }

        [DataMember]
        public string Ai2Name { get; set; }

        [DataMember]
        public string Winner { get; set; }

        [DataMember]
        public List<Goal> Goals { get; set; }

        [DataMember]
        public List<Error> Errors { get; set; }

        [DataMember]
        public List<float> MatchData { get; set; }

    }
}
