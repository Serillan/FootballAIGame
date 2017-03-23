using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.LocalSimulationBase.Models
{
    /// <summary>
    /// Represents the simulated match. Contains all the information about the 
    /// match.
    /// </summary>
    [DataContract]
    public class Match
    {
        /// <summary>
        /// Gets or sets the match information.
        /// </summary>
        /// <value>
        /// The <see cref="MatchInfo"/> containing the match information.
        /// </value>
        [DataMember]
        public MatchInfo MatchInfo { get; set; }

        /// <summary>
        /// Gets or sets the name of the first AI participating in this match.
        /// </summary>
        /// <value>
        /// The name of the first AI.
        /// </value>
        [DataMember]
        public string Ai1Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the second AI participating in this match.
        /// </summary>
        /// <value>
        /// The name of the second AI.
        /// </value>
        [DataMember]
        public string Ai2Name { get; set; }

        /// <summary>
        /// Loads the match from the specified stream.
        /// </summary>
        /// <param name="stream">The input stream from which the match is loaded.</param>
        /// <returns>The loaded <see cref="Match"/>.</returns>
        public static Match Load(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(Match));
            return serializer.ReadObject(stream) as Match;
        }

        /// <summary>
        /// Saves this instance to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to which this instance is saved.</param>
        public void Save(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(Match));
            serializer.WriteObject(stream, this);
        }
    }
}
