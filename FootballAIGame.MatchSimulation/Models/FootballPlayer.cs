using FootballAIGame.MatchSimulation.CustomDataTypes;

namespace FootballAIGame.MatchSimulation.Models
{
    /// <summary>
    /// Represents the football player in the simulation.
    /// </summary>
    public class FootballPlayer
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the speed parameter of the player. <para />
        /// The max value is 0.4.
        /// </summary>
        /// <value>
        /// The speed parameter.
        /// </value>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the possession parameter of the player. <para />
        /// The maximum value is 0.4.
        /// </summary>
        /// <value>
        /// The possession parameter.
        /// </value>
        public float Possession { get; set; }

        /// <summary>
        /// Gets or sets the precision parameter of the player. <para />
        /// The maximum value is 0.4.
        /// </summary>
        /// <value>
        /// The precision parameter.
        /// </value>
        public float Precision { get; set; }

        /// <summary>
        /// Gets or sets the kick power parameter of the player. <para />
        /// The maximum value is 0.4.
        /// </summary>
        /// <value>
        /// The kick power parameter.
        /// </value>
        public float KickPower { get; set; }

        /// <summary>
        /// Gets or sets the position of the player.
        /// </summary>
        /// <value>
        /// The position of the player.
        /// </value>
        public Vector Position { get; set; }

        /// <summary>
        /// Gets or sets the movement vector. It describes how player changes in one simulation step.
        /// </summary>
        /// <value>
        /// The <see cref="Vector"/> representing a movement.
        /// </value>
        public Vector Movement { get; set; }

        /// <summary>
        /// Gets or sets the kick vector of the player. If describes movement vector
        /// that ball would get if the kick was done with 100% precision.
        /// </summary>
        /// <value>
        /// The <see cref="Vector"/> representing a kick.
        /// </value>
        public Vector Kick { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FootballPlayer"/> class.
        /// </summary>
        public FootballPlayer(int id)
        {
            Id = id;
            Position = new Vector();
            Movement = new Vector();
            Kick = new Vector();
        }

        /// <summary>
        /// Gets the current speed in meters per second.
        /// </summary>
        /// <value>
        /// The current speed in meters per second.
        /// </value>
        public double CurrentSpeed =>
            Movement.Length * 1000 / MatchSimulator.StepInterval;

        /// <summary>
        /// Gets the maximum allowed speed of the player in meters per second.
        /// </summary>
        /// <value>
        /// The maximum speed in meters per second.
        /// </value>
        public double MaxSpeed =>
             4 + Speed * 2 / 0.4;

        /// <summary>
        /// Gets the maximum allowed kick speed describing the speed that the ball can
        /// have immediately after the player kicks it.
        /// </summary>
        /// <value>
        /// The maximum allowed kick speed.
        /// </value>
        public double MaxKickSpeed =>
            15 + KickPower * 5;
    }
}
