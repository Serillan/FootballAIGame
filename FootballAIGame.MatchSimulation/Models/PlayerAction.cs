using FootballAIGame.MatchSimulation.CustomDataTypes;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents an action of the football player in one simulation step.
    /// </summary>
    public class PlayerAction
    {
        /// <summary>
        /// Gets or sets the desired movement <see cref="Vector"/> of the football player.
        /// </summary>
        /// <value>
        /// The desired movement <see cref="Vector"/>.
        /// </value>
        public Vector Movement { get; set; }

        /// <summary>
        /// Gets or sets the desired kick <see cref="Vector"/> of the football player.
        /// </summary>
        /// <value>
        /// The desired kick <see cref="Vector"/> of the player.
        /// </value>
        public Vector Kick { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerAction"/> class.
        /// </summary>
        public PlayerAction()
        {
            Movement = new Vector();
            Kick = new Vector();
        }
    }
}
