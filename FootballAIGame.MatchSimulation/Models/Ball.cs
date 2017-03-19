using FootballAIGame.MatchSimulation.CustomDataTypes;

namespace FootballAIGame.MatchSimulation.Models
{
    public class Ball
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector Position { get; set; }

        /// <summary>
        /// Gets or sets the movement vector. It describes how ball position will
        /// change in one simulation step.
        /// </summary>
        /// <value>
        /// The movement vector.
        /// </value>
        public Vector Movement { get; set; }

        /// <summary>
        /// Gets the current speed in meters per second.
        /// </summary>
        /// <value>
        /// The current speed in meters per second.
        /// </value>
        public double CurrentSpeed =>
            Movement.Length * 1000 / MatchSimulator.StepInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ball"/> class.
        /// </summary>
        public Ball()
        {
            Position = new Vector();
            Movement = new Vector();
        }
    }
}
