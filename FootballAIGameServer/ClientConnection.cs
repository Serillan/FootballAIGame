using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.Messages;
using FootballAIGameServer.Models;

namespace FootballAIGameServer
{
    class ClientConnection
    {
        private TcpClient TcpClient { get; set; }
        private NetworkStream NetworkStream { get; set; }
        private StreamReader NetworkReader { get; set; }
        private StreamWriter NetworkWriter { get; set; }
        public bool IsActive { get; set; }
        public string PlayerName { get; set; }
        public string AiName { get; set; }

        public ClientConnection(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
            this.TcpClient.NoDelay = true;
            this.NetworkStream = tcpClient.GetStream();
            this.NetworkReader = new StreamReader(NetworkStream);
            this.NetworkWriter = new StreamWriter(NetworkStream) {AutoFlush = true};
            IsActive = false;
        }

        public async Task SendAsync(string message)
        {
            await NetworkWriter.WriteLineAsync(message);
        }

        public async Task<ClientMessage> ReceiveClientMessageAsync()
        {
            ClientMessage message;

            while (true) // while correct message is not received
            {
                var firstLine = await NetworkReader.ReadLineAsync();

                if (firstLine == "ACTION")
                {
                    var data = new byte[176];
                    await NetworkStream.ReadAsync(data, 0, data.Length);
                    message = ActionMessage.ParseMessage(data);
                    break;
                }
                else if (firstLine == "PARAMETERS")
                {
                    var data = new byte[176];
                    await NetworkStream.ReadAsync(data, 0, data.Length);
                    message = ParametersMessage.ParseMessage(data);
                    break;
                }
                else // CONNECT expected
                {
                    var tokens = firstLine.Split();
                    if (tokens.Length != 4 || tokens[0] != "LOGIN")
                    {
                        // todo send client error message
                        SendAsync("FAIL invalid message format.");
                        continue;
                    }

                    message = new LoginMessage()
                    {
                        PlayerName = tokens[1],
                        PlayerPassword = tokens[2],
                        AiName = tokens[3]

                    };
                    break;
                }
            }

            return message;
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
