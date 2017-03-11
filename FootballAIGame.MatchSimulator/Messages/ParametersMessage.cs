using System;
using FootballAIGame.MatchSimulation.SimulationEntities;

namespace FootballAIGame.MatchSimulation.Messages
{
    /// <summary>
    /// Represents a parameter message received from a client.
    /// </summary>
    /// <seealso cref="ClientMessage" />
    public class ParametersMessage : ClientMessage
    {
        /// <summary>
        /// Gets or sets the players. Players have their parameters set accordingly.
        /// </summary>
        /// <value>
        /// The players.
        /// </value>
        public FootballPlayer[] Players { get; set; }

        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <returns></returns>
        public static ParametersMessage ParseMessage(byte[] data)
        {
            var parametersMessage = new ParametersMessage();

            var floatData = new float[data.Length / 4];
            Buffer.BlockCopy(data, 0, floatData, 0, data.Length);

            var players = new FootballPlayer[11];
            for (var i = 0; i < 11; i++)
            {
                players[i] = new FootballPlayer(i)
                {
                    Speed = floatData[4*i + 0],
                    Precision = floatData[4*i + 1],
                    Possession = floatData[4*i + 2],
                    KickPower = floatData[4*i + 3]
                };
            }
            parametersMessage.Players = players;
            return parametersMessage;
        }

    }
}
