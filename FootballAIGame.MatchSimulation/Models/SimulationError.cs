using System.Runtime.Serialization;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents the simulation error.
    /// </summary>
    [DataContract]
    public class SimulationError
    {
        /// <summary>
        /// Gets or sets the match's time when the error happened.
        /// </summary>
        /// <value>
        /// The match's time when the error happened in the following format: "minutes:seconds".
        /// </value>
        [DataMember]
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets the reason why the error happened.
        /// </summary>
        /// <value>
        /// The <see cref="SimulationErrorReason"/> representing the reason behind the error.
        /// </value>
        [DataMember]
        public SimulationErrorReason Reason { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Team"/> that caused the error.
        /// </summary>
        /// <value>
        /// The <see cref="Team"/> that caused the error.
        /// </value>
        [DataMember]
        public Team Team { get; set; }

        /// <summary>
        /// Gets or sets the number of the football player that was affected by the error.
        /// </summary>
        /// <value>
        /// The affected player number if there is one exactly one player that was affected
        /// by the error; otherwise <c>null</c>.
        /// </value>
        [DataMember]
        public int? AffectedPlayerNumber { get; set; }

    }
}
