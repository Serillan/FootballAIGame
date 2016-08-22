using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.SimulationEntities
{
    public class Ball
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VectorX { get; set; }
        public float VectorY { get; set; }

        /// <summary>
        /// Returns ball current speed in metres per second.
        /// </summary>
        public double CurrentSpeed =>
            Math.Sqrt(VectorX * VectorX + VectorY * VectorY) * 1000 / MatchSimulator.StepInterval;
    }
}
