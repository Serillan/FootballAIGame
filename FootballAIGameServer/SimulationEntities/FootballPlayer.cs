using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.SimulationEntities
{
    public class FootballPlayer
    {
        public float Speed { get; set; }

        public float Possesion { get; set; }

        public float Precision { get; set; }

        public float KickPower { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float VectorX { get; set; }

        public float VectorY { get; set; }

        public float KickX { get; set; }

        public float KickY { get; set; }

        /// <summary>
        /// Returns player current speed in metres per second.
        /// </summary>
        public double CurrentSpeed =>
            VectorLength * 1000 / MatchSimulator.StepInterval;

        public double VectorLength =>
            Math.Sqrt(VectorX * VectorX + VectorY * VectorY);

        public double MaxSpeed =>
             5 + Speed * 2.5 / 0.4;
    }
}
