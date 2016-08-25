using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.CustomDataTypes;

namespace FootballAIGameServer.SimulationEntities
{
    public class PlayerAction
    {
        public Vector Movement { get; set; }

        public Vector Kick { get; set; }

        public PlayerAction()
        {
            Movement = new Vector();
            Kick = new Vector();
        }
    }
}
