using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.CustomDataTypes;

namespace FootballAIGameServer.SimulationEntities
{
    public class FootballPlayer
    {
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
        /// Gets or sets the movement vector. It describes how player position will
        /// change in one simulation step.
        /// </summary>
        /// <value>
        /// The movement vector.
        /// </value>
        public Vector Movement { get; set; }

        /// <summary>
        /// Gets or sets the kick vector of the player. If describes movement vector
        /// that ball would get if the kick was done with 100% precision.
        /// </summary>
        /// <value>
        /// The kick vector.
        /// </value>
        public Vector Kick { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FootballPlayer"/> class.
        /// </summary>
        public FootballPlayer()
        {
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

        public double MaxKickSpeed =>
            10 + KickPower * 5;
    }
}
