using System;
using System.Diagnostics;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.MatchSimulation.Messages
{
    /// <summary>
    /// Represents a parameter message received from a client.
    /// </summary>
    /// <seealso cref="IClientMessage" />
    public class ParametersMessage : IClientMessage
    {
        /// <summary>
        /// Gets or sets the array of <see cref="FootballPlayer"/>. When received from the client, players are
        /// expected to have their parameters set.
        /// </summary>
        /// <value>
        /// The array of <see cref="FootballPlayer"/>.
        /// </value>
        public FootballPlayer[] Players { get; set; }

        /// <summary>
        /// Parses the <see cref="ParametersMessage" />.
        /// </summary>
        /// <param name="data">The binary data containing the serialized <see cref="ParametersMessage" />.</param>
        /// <returns>
        /// The parsed <see cref="ParametersMessage" />
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="data"/> is not equal to 176.</exception>
        public static ParametersMessage ParseMessage(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length != 4*(4 * 11))
                throw new ArgumentOutOfRangeException(nameof(data), $"The length of the {data} must be equal to 176.");

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
