using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.Messages;

namespace FootballAIGameServer
{
    class ConnectionManager
    {
        private List<ClientConnection> Connections { get; set; }
        private List<ClientConnection> ActiveConnections { get; set; }

        private TcpListener Listener { get; set; }

        public ConnectionManager()
        {
            Listener = new TcpListener(IPAddress.Any, 50030);
            Connections = new List<ClientConnection>();
            ActiveConnections = new List<ClientConnection>();
        }

        public async Task StartListening()
        {
            Listener.Start();
            Console.WriteLine("Listening has started.");

            while (true)
            {
                var client = await Listener.AcceptTcpClientAsync();
                var connection = new ClientConnection(client);
                Console.WriteLine("New client connection established.");
                //connection.SendAsync("t");
                WaitForLogin(connection);
            }
        }

        private async Task WaitForLogin(ClientConnection connection)
        {
            while (true)
            {
                
                var clientMessage = await connection.ReceiveClientMessageAsync();
                if (!(clientMessage is LoginMessage))
                {
                    Console.WriteLine("Client has sent invalid message for connecting.");
                    // todo send error message
                }
                else
                {
                    Console.WriteLine("Player is trying to connect.");
                    // todo process connection message
                    // todo set ai name ...
                    connection.IsActive = true;
                    ActiveConnections.Add(connection);
                    connection.SendAsync("CONNECTED");
                    Console.WriteLine("New Active AI.");
                    break;
                }
            }
        }

    }
}
