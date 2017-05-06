using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.Messages;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.MatchSimulation
{
    /// <summary>
    /// Responsible for keeping TCP connection to a connected client.
    /// Provides methods for communicating with the client.
    /// </summary>
    /// <seealso cref="FootballAIGame.MatchSimulation.IClientCommunicator" /> <para />
    /// <seealso cref="System.IDisposable" />
    public class ClientConnection : IDisposable, IClientCommunicator
    {
        /// <summary>
        /// Gets or sets a value indicating whether the client is logged in.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the client is logged in; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        /// Gets a value indicating whether the connection is still on.
        /// </summary>
        /// <value>
        /// <c>true</c> if the connection is connected to the client; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                try
                {
                    if (TcpClient?.Client != null && TcpClient.Client.Connected)
                    {
                        // Detect if client disconnected
                        if (!TcpClient.Client.Poll(0, SelectMode.SelectRead)) return true;
                        var buff = new byte[1];
                        return TcpClient.Client.Receive(buff, SocketFlags.Peek) != 0;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is in a match.
        /// </summary>
        /// <value>
        /// <c>true</c> if the connection is in a match; otherwise, <c>false</c>.
        /// </value>
        public bool IsInMatch { get; set; }

        /// <summary>
        /// Gets or sets the name of the player with which the client has log on.
        /// </summary>
        /// <value>
        /// The name of the player.
        /// </value>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the name of the AI with which the client has log on.
        /// </summary>
        /// <value>
        /// The name of the AI.
        /// </value>
        public string AiName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TcpClient"/> associated with the connected client.
        /// </summary>
        /// <value>
        /// The <see cref="TcpClient"/> associated with this connection.
        /// </value>
        private TcpClient TcpClient { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="NetworkStream"/> associated with the connection.
        /// </summary>
        /// <value>
        /// The <see cref="NetworkStream"/> associated with the connection.
        /// </value>
        private NetworkStream NetworkStream { get; set; }

        /// <summary>
        /// Gets or sets the current receive task. There is always at most one receive task for
        /// receiving client messages.
        /// </summary>
        /// <value>
        /// The current receive task.
        /// </value>
        private Task<IClientMessage> CurrentReceiveTask { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnection"/> class.
        /// </summary>
        /// <param name="tcpClient">The <see cref="TcpClient"/> connected to the client.</param>
        public ClientConnection(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            TcpClient.NoDelay = true;
            NetworkStream = tcpClient.GetStream();
            IsLoggedIn = false;
        }

        /// <summary>
        /// Sends the specified message to the client asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task that represents the asynchronous send operation.</returns>
        public async Task SendAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");
            await SendAsync(bytes);
        }

        /// <summary>
        /// Tries to send the message to the client asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task that represents the asynchronous send operation. 
        /// The value of the task's result is <c>true</c> if the sending was successful; otherwise <c>false</c>.</returns>
        public async Task<bool> TrySendAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");
            return await TrySendAsync(bytes);
        }

        /// <summary>
        /// Sends the specified <see cref="GameState" /> to the client asynchronously.
        /// </summary>
        /// <param name="gameState">The state of the game.</param>
        /// <param name="team">The <see cref="Team" /> belonging to this connection.</param>
        /// <returns>The task that represents the asynchronous send operation. </returns>
        public async Task SendAsync(GameState gameState, Team team)
        {
            var data = new float[92];

            data[0] = (float)gameState.Ball.Position.X;
            data[1] = (float)gameState.Ball.Position.Y;
            data[2] = (float)gameState.Ball.Movement.X;
            data[3] = (float)gameState.Ball.Movement.Y;

            if (team == Team.FirstPlayer)
            {
                for (var i = 0; i < 22; i++)
                {
                    data[4 + 4 * i + 0] = (float)gameState.FootballPlayers[i].Position.X;
                    data[4 + 4 * i + 1] = (float)gameState.FootballPlayers[i].Position.Y;
                    data[4 + 4 * i + 2] = (float)gameState.FootballPlayers[i].Movement.X;
                    data[4 + 4 * i + 3] = (float)gameState.FootballPlayers[i].Movement.Y;

                }
            }
            else
            {
                for (var i = 0; i < 11; i++)
                {
                    data[4 + 4 * i + 0] = (float)gameState.FootballPlayers[i + 11].Position.X;
                    data[4 + 4 * i + 1] = (float)gameState.FootballPlayers[i + 11].Position.Y;
                    data[4 + 4 * i + 2] = (float)gameState.FootballPlayers[i + 11].Movement.X;
                    data[4 + 4 * i + 3] = (float)gameState.FootballPlayers[i + 11].Movement.Y;
                }
                for (var i = 11; i < 22; i++)
                {
                    data[4 + 4 * i + 0] = (float)gameState.FootballPlayers[i - 11].Position.X;
                    data[4 + 4 * i + 1] = (float)gameState.FootballPlayers[i - 11].Position.Y;
                    data[4 + 4 * i + 2] = (float)gameState.FootballPlayers[i - 11].Movement.X;
                    data[4 + 4 * i + 3] = (float)gameState.FootballPlayers[i - 11].Movement.Y;
                }
            }

            var byteArray = new byte[data.Length * 4 + 4 + 1];
            var numArray = new[] { gameState.Step };

            Buffer.BlockCopy(numArray, 0, byteArray, 0, 4);
            byteArray[4] = gameState.IsKickOff ? (byte)1 : (byte)0;
            Buffer.BlockCopy(data, 0, byteArray, 5, data.Length * 4);
            await SendAsync(byteArray);
        }

        /// <summary>
        /// Tries to send the specified game state to the client asynchronously.
        /// </summary>
        /// <param name="gameState">The state of the game.</param>
        /// <param name="team">The <see cref="Team"/> belonging to this connection.</param>
        /// <returns>The task that represents the asynchronous send operation. 
        /// The value of the task's result is <c>true</c> if the sending was successful; otherwise <c>false</c>.</returns>
        public async Task<bool> TrySendAsync(GameState gameState, Team team)
        {
            try
            {
                await SendAsync(gameState, team);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends the specified data to the client asynchronously.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The task that represents the asynchronous send operation.</returns>
        public async Task SendAsync(byte[] data)
        {
            await NetworkStream.WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Tries to send the specified data to the client asynchronously.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The task that represents the asynchronous send operation. 
        /// The value of the task's result is <c>true</c> if the sending was successful; otherwise <c>false</c>.</returns>
        public async Task<bool> TrySendAsync(byte[] data)
        {
            try
            {
                await SendAsync(data);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Receives a client message asynchronously.
        /// The task's result is null if the connection is dropped.
        /// </summary>
        /// <returns>The task that represents the asynchronous receive operation. 
        /// The value of the task's result is null if the connection is dropped; otherwise,
        /// the received <see cref="IClientMessage"/>.</returns>
        public async Task<IClientMessage> ReceiveClientMessageAsync()
        {
            if (CurrentReceiveTask == null || CurrentReceiveTask.IsCanceled || CurrentReceiveTask.IsCompleted ||
                CurrentReceiveTask.IsFaulted)
                CurrentReceiveTask = StartReceivingClientMessageAsync();

            try
            {
                return await CurrentReceiveTask;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Reads the next line asynchronously.
        /// The task's result is null if the connection is dropped.
        /// </summary>
        /// <returns>The task that represents the asynchronous read operation. The value of the task's result
        /// is null if the connection is dropped; otherwise, the received line.</returns>
        public async Task<string> ReadLineAsync()
        {
            var bytes = new List<byte>();
            var buffer = new byte[1];

            while (true)
            {
                try
                {
                    await NetworkStream.ReadAsync(buffer, 0, 1);
                }
                catch (Exception)
                {
                    return null;
                }

                if (buffer[0] == '\n')
                    break;
                bytes.Add(buffer[0]);
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Starts a new receiving of a <see cref="IClientMessage"/> if there is not already one receiving.
        /// The task's result is null if the connection is dropped.
        /// </summary>
        /// <returns>The task that represents the asynchronous receive operation. 
        /// The value of the task's result is null if the connection is dropped; otherwise,
        /// the received <see cref="IClientMessage"/>.</returns>
        private async Task<IClientMessage> StartReceivingClientMessageAsync()
        {
            IClientMessage message;

            while (true) // while correct message is not received
            {
                var firstLine = await ReadLineAsync();//await NetworkReader.ReadLineAsync();

                if (firstLine.Length >= 6 && firstLine.Substring(firstLine.Length - 6) == "ACTION")
                {

                    var data = new byte[180];
                    await NetworkStream.ReadAsync(data, 0, data.Length);
                    message = ActionMessage.ParseMessage(data);
                    break;
                }
                else if (firstLine.Length >= 10 && firstLine.Substring(firstLine.Length - 10) == "PARAMETERS")
                {
                    var data = new byte[176];
                    await NetworkStream.ReadAsync(data, 0, data.Length);
                    message = ParametersMessage.ParseMessage(data);
                    break;
                }
                else // LOGIN expected
                {
                    var tokens = firstLine.Split();
                    if (tokens.Length != 4 || (tokens.Length > 0 && tokens[0] != "LOGIN"))
                    {
                        //Console.WriteLine($"{PlayerName} - received line: {firstLine}");
                        //SendAsync("FAIL invalid message format.");
                        continue;
                    }

                    message = new LoginMessage()
                    {
                        PlayerName = tokens[1],
                        AIName = tokens[2],
                        AccessKey = tokens[3]
                    };
                    break;
                }
            }

            return message;
        }

        /// <summary>
        /// Closes the connection. Releases all allocated resources.
        /// </summary>
        public void Dispose()
        {
            TcpClient.Close();
        }

        /// <summary>
        /// Receives the <see cref="ActionMessage"/> asynchronously.
        /// The task's result is null if the connection is dropped.
        /// </summary>
        /// <param name="step">The simulation step which a received <see cref="ActionMessage"/> must have.</param>
        /// <returns>
        /// The task that represents the asynchronous receive operation. The value of the task's result is null 
        /// if the connection is dropped; otherwise, the received <see cref="ActionMessage"/>.
        /// </returns>
        public async Task<ActionMessage> ReceiveActionMessageAsync(int step)
        {
            while (true)
            {
                var message = await ReceiveClientMessageAsync();

                if (message == null) //connection dropped
                    return null;

                var actionMessage = message as ActionMessage;

                if (actionMessage == null)
                {
                    //Console.WriteLine("Invalid message received.");
                    continue;
                }

                if (actionMessage.Step == step)
                    return actionMessage;
                else if (actionMessage.Step > step)
                {
                    //Console.WriteLine($"Wrong action received! Received {actionMessage.Step} instead of {step}");
                    return null;
                }
                else if (actionMessage.Step < step)
                {
                    //Console.WriteLine($"Wrong action received! Received {actionMessage.Step} instead of {step}");
                    return null;
                }
                else
                {
                    //Console.WriteLine($"Wrong action received! actionMessage.Step == null, expected step - {step}");
                }
            }
        }

    }
}
