using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.CustomDataTypes;

namespace FootballAIGameServer.SimulationEntities
{
    public class Ball
    {
        public Vector Position { get; set; }
        public Vector Movement { get; set; }

        /// <summary>
        /// Returns ball current speed in metres per second.
        /// </summary>
        public double CurrentSpeed =>
            Movement.Length * 1000 / MatchSimulator.StepInterval;

        public Ball()
        {
            Position = new Vector();
            Movement = new Vector();
        }
    }
}
