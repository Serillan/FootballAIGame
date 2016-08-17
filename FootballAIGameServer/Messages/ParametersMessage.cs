using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer.Messages
{
    class ParametersMessage : ClientMessage
    {
        public FootballPlayer[] Players { get; set; }

        public static ParametersMessage ParseMessage(byte[] data)
        {
            var parametersMessage = new ParametersMessage();

            var floatData = new float[data.Length / 4];
            Buffer.BlockCopy(data, 0, floatData, 0, data.Length);

            var players = new FootballPlayer[11];
            for (int i = 0; i < 11; i++)
            {
                players[i].Speed = floatData[4 * i + 0];
                players[i].Precision = floatData[4 * i + 1];
                players[i].Possesion = floatData[4 * i + 2];
                players[i].KickPower = floatData[4 * i + 3];
            }

            return parametersMessage;
        }
    }

}
