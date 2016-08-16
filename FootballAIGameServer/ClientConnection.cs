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
        public Player Player { get; set; }
        public string AiName { get; set; }

        public ClientConnection(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
            this.TcpClient.NoDelay = true;
            this.NetworkStream = tcpClient.GetStream();
            this.NetworkReader = new StreamReader(NetworkStream);
            this.NetworkWriter = new StreamWriter(NetworkStream);
            this.NetworkWriter.AutoFlush = true;
            IsActive = false;
        }

        public async Task SendAsync(string message)
        {
            await NetworkWriter.WriteLineAsync(message);
            NetworkWriter.Flush();
        }

        public async Task<ClientMessage> ReceiveClientMessageAsync()
        {
            ClientMessage message;

            while (true) // while correct message is not received
            {
                var firstLine = await NetworkReader.ReadLineAsync();

                if (firstLine == "ACTION")
                {
                    message = new ActionMessage();

                    break;
                }
                else if (firstLine == "PARAMETERS")
                {
                    message = new ParametersMessage();

                    break;
                }
                else // CONNECT expected
                {
                    var tokens = firstLine.Split();
                    if (tokens.Length != 4 || tokens[0] != "LOGIN")
                    {
                        // todo send client error message
                        continue;
                    }

                    message = new LoginMessage();

                    break;
                }
            }

            return message;
        }

    }
}
