using System;
using FootballAIGame.MatchSimulation.CustomDataTypes;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.MatchSimulation.Messages
{
    /// <summary>
    /// Represents an action message received from a client. Holds the
    /// actions of the football players in some simulation step.
    /// </summary>
    /// <seealso cref="IClientMessage" />
    public class ActionMessage : IClientMessage
    {
        /// <summary>
        /// Gets or sets the football players' actions.
        /// </summary>
        /// <value>
        /// The players' actions.
        /// </value>
        public PlayerAction[] PlayersActions { get; set; }

        /// <summary>
        /// Gets or sets the simulation step to which this action belongs.
        /// </summary>
        /// <value>
        /// The simulation step to which this action belongs.
        /// </value>
        public int Step { get; set; }

        /// <summary>
        /// Parses the <see cref="ActionMessage" />.
        /// </summary>
        /// <param name="data">The binary data containing the serialized <see cref="ActionMessage" />.</param>
        /// <returns>
        /// The parsed <see cref="ActionMessage" />
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="data"/> is not equal to 180.</exception>
        public static ActionMessage ParseMessage(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length != 4 * (4 * 11 + 1))
                throw new ArgumentOutOfRangeException(nameof(data), $"The length of the {data} must be equal to 180.");

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
