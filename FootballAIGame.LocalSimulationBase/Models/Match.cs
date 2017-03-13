using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.LocalSimulationBase.Models
{
    [DataContract]
    public class Match
    {
        [DataMember]
        public MatchInfo MatchInfo { get; set; }

        [DataMember]
        public string Ai1Name { get; set; }

        [DataMember]
        public string Ai2Name { get; set; }

        public static Match Load(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(Match));
            return serializer.ReadObject(stream) as Match;
        }

        public void Save(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(Match));
            serializer.WriteObject(stream, this);
        }
    }
}
