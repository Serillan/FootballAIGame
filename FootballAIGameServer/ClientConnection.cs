using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FootballAIGameServer.Messages;
using FootballAIGameServer.Models;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer
{
    /// <summary>
    /// Responsible for keeping TCP connection to the connected client.
    /// Provides methods for communicating with the client.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class ClientConnection : IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the connection is active.
        /// Connection becomes active when the client successfully logs in.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
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
        /// Gets or sets a value indicating whether this instance is in match.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in match; otherwise, <c>false</c>.
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
        /// Gets or sets the TCP client associated with the connected client.
        /// </summary>
        /// <value>
        /// The TCP client.
        /// </value>
        private TcpClient TcpClient { get; set; }

        /// <summary>
        /// Gets or sets the network stream associated with the connected client.
        /// </summary>
        /// <value>
        /// The network stream.
        /// </value>
        private NetworkStream NetworkStream { get; set; }

        /// <summary>
        /// Gets or sets the current receive task. There is always one receive task for
        /// receiving client messages. After the message was received new receive task
        /// is created when somebody uses <see cref="ReceiveClientMessageAsync"/> method.
        /// </summary>
        /// <value>
        /// The current receive task.
        /// </value>
        private Task<ClientMessage> CurrentReceiveTask { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnection"/> class.
        /// </summary>
        /// <param name="tcpClient">The TCP client connected to the client application.</param>
        public ClientConnection(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            TcpClient.NoDelay = true;
            NetworkStream = tcpClient.GetStream();
            IsActive = false;
        }

        /// <summary>
        /// Pings the connected client several times and computes average round trip time in milliseconds.
        /// If client doesn't allow pinging, returns default round trip time.
        /// </summary>
        /// <returns>The average round trip time if client allows pings; otherwise returns the default 
        /// round trip time.</returns>
        public int PingTimeAverage()
        {
            long totalTime = 0;
            var timeout = 500;
            var echoNum = 5;
            var succNum = 0;
            var pingSender = new Ping();

            for (int i = 0; i < echoNum; i++)
            {
                var reply = pingSender.Send(((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address, timeout);
                if (reply.Status != IPStatus.BadDestination && reply.Status != IPStatus.TimedOut)
                {
                    succNum++;
                    totalTime += reply.RoundtripTime;
                }
            }
            if (succNum == 0)
                return 100; // default
            return (int)(totalTime / succNum);
        }

        /// <summary>
        /// Sends message to the client asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");
            await SendAsync(bytes);
        }

        /// <summary>
        /// Sends the specified game state to the client asynchronously.
        /// </summary>
        /// <param name="gameState">State of the game.</param>
        /// <param name="playerNumber">1 if first 11 players from the given game state are
        /// client's players; otherwise 2 (seconds half are client's players)</param>
        /// <returns></returns>
        public async Task SendAsync(GameState gameState, int playerNumber)
        {
            var data = new float[92];

            data[0] = (float)gameState.Ball.Position.X;
            data[1] = (float)gameState.Ball.Position.Y;
            data[2] = (float)gameState.Ball.Movement.X;
            data[3] = (float)gameState.Ball.Movement.Y;

            if (playerNumber == 1)
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

            var byteArray = new byte[data.Length * 4 + 4];
            var numArray = new int[1] {gameState.Step};

            Buffer.BlockCopy(numArray, 0, byteArray, 0, 4);
            Buffer.BlockCopy(data, 0, byteArray, 4, data.Length * 4);
            await SendAsync(byteArray);
        }

        /// <summary>
        /// Sends the specified data to the client asynchronously.
        /// </summary>
        /// <param name="data">The data.</param>
        public async Task SendAsync(byte[] data)
        {
            await NetworkStream.WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Receives the client message asynchronously.
        /// </summary>
        /// <returns>The next received client message.</returns>
        public async Task<ClientMessage> ReceiveClientMessageAsync()
        {
            if (CurrentReceiveTask == null || CurrentReceiveTask.IsCanceled || CurrentReceiveTask.IsCompleted ||
                CurrentReceiveTask.IsFaulted)
                CurrentReceiveTask = ReceiveClientMessageAsyncTask();

            return await CurrentReceiveTask;
        }

        /// <summary>
        /// Reads the next line asynchronously.
        /// </summary>
        /// <returns>The read line.</returns>
        public async Task<string> ReadLineAsync()
        {
            var bytes = new List<byte>();
            var buffer = new byte[1];

            while (true)
            {
                await NetworkStream.ReadAsync(buffer, 0, 1);
                if (buffer[0] == (int)'\n')
                    break;
                bytes.Add(buffer[0]);
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Receives the client message asynchronously task. There is always only
        /// one receiving at any moment.
        /// </summary>
        /// <returns>The next received client message.</returns>
        private async Task<ClientMessage> ReceiveClientMessageAsyncTask()
        {
            ClientMessage message;

            while (true) // while correct message is not received
            {
                //Console.WriteLine($"{PlayerName} - receiving line");
                var firstLine = await ReadLineAsync();//await NetworkReader.ReadLineAsync();
                //Console.WriteLine($"{PlayerName} - received line: {firstLine}");

                if (firstLine.Length >= 6 && firstLine.Substring(firstLine.Length - 6) == "ACTION")
                {
                    if (firstLine != "ACTION")
                        Console.WriteLine("line ending with action");

                    var data = new byte[180];
                    await NetworkStream.ReadAsync(data, 0, data.Length);
                    //Console.WriteLine($"{PlayerName} - received action");
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
                    if (tokens.Length != 3 || (tokens.Length > 0 && tokens[0] != "LOGIN"))
                    {
                        Console.WriteLine($"{PlayerName} - received line: {firstLine}");
                        //SendAsync("FAIL invalid message format.");
                        continue;
                    }

                    message = new LoginMessage()
                    {
                        PlayerName = tokens[1],
                        AiName = tokens[2]

                    };
                    break;
                }
            }

            return message;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            TcpClient.Close();
        }

        /// <summary>
        /// Receives the action message asynchronously.
        /// </summary>
        /// <param name="step">The expected simulation step of the action message.</param>
        /// <returns>The next received action client message with the specified step.</returns>
        public async Task<ActionMessage> ReceiveActionMessageAsync(int step)
        {
            while (true)
            {
                var message = await ReceiveClientMessageAsync();
                var actionMessage = message as ActionMessage;
                if (actionMessage?.Step == step)
                    return actionMessage;
                else if (actionMessage?.Step > step)
                    return null;
            }
        }
    }
}
