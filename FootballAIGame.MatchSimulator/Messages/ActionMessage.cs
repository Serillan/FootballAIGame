using System;
using FootballAIGame.MatchSimulation.CustomDataTypes;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.MatchSimulation.Messages
{
    /// <summary>
    /// Represents an action message received from a client.
    /// </summary>
    /// <seealso cref="ClientMessage" />
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
        /// Gets or sets the simulation step of this action.
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <returns></returns>
        public static ActionMessage ParseMessage(byte[] data)
        {
            var actionMessage = new ActionMessage();

            var intData = new int[1];
            var floatData = new float[(data.Length-4)/4];
            
            Buffer.BlockCopy(data, 0, intData, 0, 4);
            Buffer.BlockCopy(data, 4, floatData, 0, data.Length - 4);

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
            actionMessage.Step = intData[0];
            return actionMessage;
        }
    }
}
