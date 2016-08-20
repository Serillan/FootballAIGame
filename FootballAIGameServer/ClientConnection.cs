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
    public class ClientConnection
    {
        private TcpClient TcpClient { get; set; }
        private NetworkStream NetworkStream { get; set; }
        private StreamReader NetworkReader { get; set; }
        private StreamWriter NetworkWriter { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }

        public bool IsActive { get; set; }
        public string PlayerName { get; set; }
        public string AiName { get; set; }

        private Task<ClientMessage> CurrentReceivetask { get; set; }

        public ClientConnection(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
            this.TcpClient.NoDelay = true;
            this.NetworkStream = tcpClient.GetStream();
            this.NetworkReader = new StreamReader(NetworkStream);
            this.NetworkWriter = new StreamWriter(NetworkStream) { AutoFlush = true };
            this.CancellationTokenSource = new CancellationTokenSource();
            IsActive = false;
        }

        public int PingTimeAverage()
        {
            long totalTime = 0;
            var timeout = 330;
            var echoNum = 3;
            var pingSender = new Ping();

            for (int i = 0; i < echoNum; i++)
            {
                var reply = pingSender.Send(((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address, timeout);
                if (reply.Status != IPStatus.BadDestination)
                    totalTime += reply.RoundtripTime;
            }
            return (int)(totalTime / echoNum);
        }

        public async Task SendAsync(string message)
        {
            await NetworkWriter.WriteLineAsync(message);
        }

        public async Task SendAsync(GameState gameState)
        {
            var data = new float[92];

            data[0] = gameState.Ball.X;
            data[1] = gameState.Ball.Y;
            data[2] = gameState.Ball.VectorX;
            data[3] = gameState.Ball.VectorY;

            for (var i = 0; i < 22; i++)
            {
                data[4 + 4 * i + 0] = gameState.FootballPlayers[i].X;
                data[4 + 4 * i + 1] = gameState.FootballPlayers[i].Y;
                data[4 + 4 * i + 2] = gameState.FootballPlayers[i].VectorX;
                data[4 + 4 * i + 3] = gameState.FootballPlayers[i].VectorY;

            }

            var byteArray = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);

            await Send(byteArray);
        }

        public async Task Send(byte[] data)
        {
            await NetworkStream.WriteAsync(data, 0, data.Length);
        }

        public async Task<ClientMessage> ReceiveClientMessageAsync()
        {
            if (CurrentReceivetask == null || CurrentReceivetask.IsCanceled || CurrentReceivetask.IsCompleted ||
                CurrentReceivetask.IsFaulted)
                CurrentReceivetask = ReceiveClientMessageAsyncTask();

            return await CurrentReceivetask;
        }

        private async Task<ClientMessage> ReceiveClientMessageAsyncTask()
        {
            ClientMessage message;

            while (true) // while correct message is not received
            {
                var firstLine = await NetworkReader.ReadLineAsync();

                if (firstLine == "ACTION")
                {
                    var data = new byte[176];
                    await NetworkStream.ReadAsync(data, 0, data.Length, CancellationTokenSource.Token);
                    message = ActionMessage.ParseMessage(data);
                    break;
                }
                else if (firstLine == "PARAMETERS")
                {
                    var data = new byte[176];
                    await NetworkStream.ReadAsync(data, 0, data.Length, CancellationTokenSource.Token);
                    message = ParametersMessage.ParseMessage(data);
                    break;
                }
                else // CONNECT expected
                {
                    var tokens = firstLine.Split();
                    if (tokens.Length != 3 || tokens[0] != "LOGIN")
                    {
                        // todo send client error message
                        SendAsync("FAIL invalid message format.");
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

        public void CancelCurrentReceiving()
        {
            CancellationTokenSource.Cancel();
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
                    {
                        // Detect if client disconnected
                        if (TcpClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (TcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                            {
                                // Client disconnected
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                        return true;
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
    }
}
