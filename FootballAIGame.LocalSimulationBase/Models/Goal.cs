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
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets the name of the football player who scored.
        /// </summary>
        [DataMember]
        public string PlayerName { get; set; }
    }
}
