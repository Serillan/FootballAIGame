using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.Messages
{
    public class ActionMessage : ClientMessage
    {
        public PlayerAction[] PlayerActions { get; set; }

        public static ActionMessage ParseMessage(byte[] data)
        {
            var actionMessage = new ActionMessage();

            var floatData = new float[data.Length/4];
            Buffer.BlockCopy(data, 0, floatData, 0, data.Length);

            var playerActions = new PlayerAction[11];
            for (var i = 0; i < 11; i++)
            {
                playerActions[i] = new PlayerAction
                {
                    VectorX = floatData[4*i + 0],
                    VectorY = floatData[4*i + 1],
                    KickX = floatData[4*i + 2],
                    KickY = floatData[4*i + 3]
                };
            }
            actionMessage.PlayerActions = playerActions;
            return actionMessage;
        }
    }

    public class PlayerAction
    {
        public float VectorX { get; set; }

        public float VectorY { get; set; }

        public float KickX { get; set; }

        public float KickY { get; set; }

        public double VectorLength =>
            Math.Sqrt(VectorX * VectorX + VectorY * VectorY);

        public double KickVectorLength =>
            Math.Sqrt(KickX * KickX + KickY * KickY);
    }
}
