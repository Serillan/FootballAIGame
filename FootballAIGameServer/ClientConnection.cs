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
        private Task<ClientMessage> CurrentReceiveTask { get; set; }

        public bool IsActive { get; set; }
        public bool IsInMatch { get; set; }
        public string PlayerName { get; set; }
        public string AiName { get; set; }

        public ClientConnection(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            TcpClient.NoDelay = true;
            NetworkStream = tcpClient.GetStream();
            IsActive = false;
        }

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

        public async Task SendAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");
            await Send(bytes);
        }

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

            var byteArray = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
            var back = new float[92];
            Buffer.BlockCopy(byteArray, 0, back, 0, byteArray.Length);
            await Send(byteArray);
        }

        public async Task Send(byte[] data)
        {
            await NetworkStream.WriteAsync(data, 0, data.Length);
        }

        public async Task<ClientMessage> ReceiveClientMessageAsync()
        {
            if (CurrentReceiveTask == null || CurrentReceiveTask.IsCanceled || CurrentReceiveTask.IsCompleted ||
                CurrentReceiveTask.IsFaulted)
                CurrentReceiveTask = ReceiveClientMessageAsyncTask();

            return await CurrentReceiveTask;
        }

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

                    var data = new byte[176];
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
        
    }
}
