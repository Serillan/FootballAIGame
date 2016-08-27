using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.CustomDataTypes;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer.Messages
{
    /// <summary>
    /// Represents an action message received from a client.
    /// </summary>
    /// <seealso cref="FootballAIGameServer.Messages.ClientMessage" />
    public class ActionMessage : ClientMessage
    {
        /// <summary>
        /// Gets or sets the players actions.
        /// </summary>
        /// <value>
        /// The players actions.
        /// </value>
        public PlayerAction[] PlayersActions { get; set; }

        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <returns></returns>
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
                    Movement = new Vector(floatData[4*i + 0], floatData[4 * i + 1]),
                    Kick = new Vector(floatData[4*i + 2], floatData[4*i + 3])
                };
            }
            actionMessage.PlayersActions = playerActions;
            return actionMessage;
        }
    }
}
