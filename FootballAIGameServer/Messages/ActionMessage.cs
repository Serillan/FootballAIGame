using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.Messages
{
    class ActionMessage : ClientMessage
    {
        public PlayerAction[] PlayerActions { get; set; }

        public static ActionMessage ParseMessage(byte[] data)
        {
            var actionMessage = new ActionMessage();

            var floatData = new float[data.Length/4];
            Buffer.BlockCopy(data, 0, floatData, 0, data.Length);

            var playerActions = new PlayerAction[11];
            for (int i = 0; i < 11; i++)
            {
                playerActions[i].VectorX = floatData[4*i + 0];
                playerActions[i].VectorY = floatData[4 * i + 1];
                playerActions[i].KickX = floatData[4 * i + 2];
                playerActions[i].KickX = floatData[4 * i + 3];
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
    }
}
