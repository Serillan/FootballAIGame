using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer.Messages
{
    public class ParametersMessage : ClientMessage
    {
        public FootballPlayer[] Players { get; set; }

        public static ParametersMessage ParseMessage(byte[] data)
        {
            var parametersMessage = new ParametersMessage();

            var floatData = new float[data.Length / 4];
            Buffer.BlockCopy(data, 0, floatData, 0, data.Length);

            var players = new FootballPlayer[11];
            for (var i = 0; i < 11; i++)
            {
                players[i] = new FootballPlayer
                {
                    Speed = floatData[4*i + 0],
                    Precision = floatData[4*i + 1],
                    Possesion = floatData[4*i + 2],
                    KickPower = floatData[4*i + 3]
                };
            }
            parametersMessage.Players = players;
            return parametersMessage;
        }

    }
}
