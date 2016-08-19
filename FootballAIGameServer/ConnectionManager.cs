using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FootballAIGameServer.Messages;
using FootballAIGameServer.Models;

namespace FootballAIGameServer
{
    public class ConnectionManager
    {
        public List<ClientConnection> Connections { get; set; }
        public List<ClientConnection> ActiveConnections { get; set; }
        public const int GameServerPort = 50030;

        public List<ClientConnection> WantsToPlayConnections { get; set; }

        private static ConnectionManager _instance; // singleton instance

        public static ConnectionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConnectionManager();
                }
                return _instance;
            }
        }

        private TcpListener Listener { get; set; }

        private ConnectionManager()
        {
            Listener = new TcpListener(IPAddress.Any, GameServerPort);
            Connections = new List<ClientConnection>();
            ActiveConnections = new List<ClientConnection>();
            WantsToPlayConnections = new List<ClientConnection>();
        }

        public async Task StartCheckingConnections()
        {
            while (true)
            {
                await Task.Delay(3000);
                lock (Connections)
                {
                    var toBeRemovedConnections = new List<ClientConnection>();
                    foreach (var clientConnection in Connections)
                    {
                        if (!clientConnection.IsConnected)
                        {
                            clientConnection.IsActive = false;
                            toBeRemovedConnections.Add(clientConnection);
                            Console.WriteLine($"Player {clientConnection.PlayerName} with AI " +
                                              $"{clientConnection.AiName} has disconnected.");
                        }
                    }

                    // remove from db
                    using (var context = new ApplicationDbContext())
                    {
                        foreach (var toBeRemovedConnection in toBeRemovedConnections)
                        {
                            var player =
                                context.Players.SingleOrDefault(p => p.Name == toBeRemovedConnection.PlayerName);
                            var newAis = player.ActiveAis.Split(';').Where(s => s != toBeRemovedConnection.AiName);
                            player.ActiveAis = String.Join(";", newAis);

                            player.PlayerState = PlayerState.Idle; // todo error message
                            ConnectionManager.Instance.WantsToPlayConnections.Remove(toBeRemovedConnection);

                            if (player.SelectedAi == toBeRemovedConnection.AiName)
                                player.SelectedAi = "";

                            if (player.ActiveAis == "")
                                player.ActiveAis = null;
                        }

                        context.SaveChanges();
                    }

                    Connections.RemoveAll(c => toBeRemovedConnections.Contains(c));
                    ActiveConnections.RemoveAll(c => c.IsActive == false);
                }
            }
        }

        public async Task StartListening()
        {
            Listener.Start();
            StartCheckingConnections();
            Console.WriteLine("Listening has started.");

            while (true)
            {
                var client = await Listener.AcceptTcpClientAsync();
                var connection = new ClientConnection(client);
                Connections.Add(connection);
                Console.WriteLine("New client connection established.");
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
                    connection.SendAsync("FAIL invalid message format.");
                }
                else
                {
                    if (await ProcessLoginMessageAsync((LoginMessage) clientMessage, connection))
                    {
                        connection.IsActive = true;
                        ActiveConnections.Add(connection);
                        connection.SendAsync("CONNECTED");
                        Console.WriteLine($"Player {connection.PlayerName} with ai {connection.AiName} has log on.");
                        break;
                    }
                }
            }
        }

        private async Task<bool> ProcessLoginMessageAsync(LoginMessage message, ClientConnection connection)
        {
            using (var context = new ApplicationDbContext())
            {
                var player = await context.Players
                    .Include(u => u.User)
                    .FirstOrDefaultAsync(p => p.Name == message.PlayerName);
                if (player == null)
                {
                    connection.SendAsync("Invalid Playername");
                    return false;
                }
                if (player.ActiveAis == null)
                    player.ActiveAis = message.AiName;
                else
                {
                    if (player.ActiveAis.Split(';').Contains(message.AiName))
                    {
                        await connection.SendAsync("AI name is already being used.");
                        return false;
                    }
                    player.ActiveAis += ";" + message.AiName;
                }
                connection.AiName = message.AiName;
                connection.PlayerName = message.PlayerName;
                context.SaveChanges();
            }

            return true;
        }

    }
}
