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
        public float Speed { get; set; }

        public float Possesion { get; set; }

        public float Precision { get; set; }

        public float KickPower { get; set; }

        public Vector Position { get; set; }

        public Vector Movement { get; set; }

        public Vector Kick { get; set; }

        public FootballPlayer()
        {
            Position = new Vector();
            Movement = new Vector();
            Kick = new Vector();
        }

        /// <summary>
        /// Returns player current speed in metres per second.
        /// </summary>
        public double CurrentSpeed =>
            Movement.Length * 1000 / MatchSimulator.StepInterval;

        public double MaxSpeed =>
             5 + Speed * 2.5 / 0.4;
    }
}
