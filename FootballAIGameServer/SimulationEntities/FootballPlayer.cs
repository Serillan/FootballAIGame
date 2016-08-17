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
    }
}
