using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGame.LocalSimulationBase.Models
{
    [DataContract]
    public class Goal
    {
        [DataMember]
        public string AiName { get; set; }

        [DataMember]
        public DateTime Time { get; set; }

        [DataMember]
        public int PlayerNumber { get; set; }
    }
}
