using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FootballAIGameServer.Messages;

namespace FootballAIGameServer
{

    public delegate Task PlayerDisconectedHandler(ClientConnection connection);
    public delegate Task<bool> AuthenticationHandler(LoginMessage message, ClientConnection connection);


    /// <summary>
    /// Responsible for keeping all the client connections.
    /// Provides method to start listening for new connections, and public properties
    /// to access those connections. <para />
    /// It is singleton class. Use Instance static property to get the instance.
    /// </summary>
    public class ConnectionManager
    {
        /// <summary>
        /// The port on which connection manager listens for connections. Also this port is used
        /// for communication with connected clients.
        /// </summary>
        public const int GameServerPort = 50030;

        /// <summary>
        /// The interval in milliseconds of checking that clients are still connected and also for sending keep alive
        /// packets to clients that are not currently in a match.
        /// </summary>
        public const int CheckConnectionsInterval = 5000; // [ms]

        /// <summary>
        /// Gets or sets the connected connections.
        /// </summary>
        /// <value>
        /// The connections.
        /// </value>
        public List<ClientConnection> Connections { get; set; }

        /// <summary>
        /// Gets or sets the active connections. Connection becomes active when the client successfully logs in.
        /// </summary>
        /// <value>
        /// The active connections.
        /// </value>
        public List<ClientConnection> ActiveConnections { get; set; }

        public AuthenticationHandler AuthenticationHandler { get; set; }

        public PlayerDisconectedHandler PlayerDisconectedHandler { get; set; }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ConnectionManager Instance => _instance ?? (_instance = new ConnectionManager());
        private static ConnectionManager _instance; // singleton instance

        /// <summary>
        /// Gets or sets the TCP listener that listens for new clients.
        /// </summary>
        /// <value>
        /// The listener.
        /// </value>
        private TcpListener Listener { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class. <para/>
        /// Prevents a default instance of the <see cref="ConnectionManager"/> class from being created.
        /// </summary>
        private ConnectionManager()
        {
            Listener = new TcpListener(IPAddress.Any, GameServerPort);
            Connections = new List<ClientConnection>();
            ActiveConnections = new List<ClientConnection>();
            AuthenticationHandler = DefaultAuthenticate;
        }

        /// <summary>
        /// Starts the listening for new AI clients. After the client connects, he is added to
        /// the <see cref="Connections"/> collection and he remains there while the connection is opened.
        /// </summary>
        public async Task StartListeningAsync()
        {
            Listener.Start();

            StartCheckingConnections();

            while (true)
            {
                var client = await Listener.AcceptTcpClientAsync();
                var connection = new ClientConnection(client);
                lock (Connections)
                {
                    Connections.Add(connection);
                }
                Console.WriteLine("New client connection established.");
                WaitForLogin(connection);
            }
        }

        /// <summary>
        /// Starts checking that clients from <see cref="Connections"/> are still connected 
        /// and keep sending keep alive packets to clients that are not currently in a match. <para />
        /// The interval of doing it is specified by <see cref="CheckConnectionsInterval"/>.
        /// </summary>
        /// <returns></returns>
        public async void StartCheckingConnections()
        {
            while (true)
            {
                await Task.Delay(CheckConnectionsInterval);

                var runningTasks = new List<Task>();
                var toBeRemovedConnections = new List<ClientConnection>();

                lock (Connections)
                {

                    foreach (var clientConnection in Connections)
                    {
                        if (!clientConnection.IsConnected)
                        {
                            if (clientConnection.IsActive)
                                Console.WriteLine($"Player {clientConnection.PlayerName} with AI " +
                                              $"{clientConnection.AiName} has disconnected.");
                            clientConnection.IsActive = false;

                            // delegate call
                            runningTasks.Add(PlayerDisconectedHandler?.Invoke(clientConnection));

                            toBeRemovedConnections.Add(clientConnection);

                        }
                       // else if (!clientConnection.IsInMatch && clientConnection.IsActive)
                        {
                            runningTasks.Add(clientConnection.TrySendAsync("keepalive"));
                        }
                    }

                    Connections.RemoveAll(c => toBeRemovedConnections.Contains(c));

                }

                await Task.WhenAll(runningTasks);

                toBeRemovedConnections.ForEach(c => c.Dispose());
               

                lock (ActiveConnections)
                {
                    ActiveConnections.RemoveAll(c => c.IsActive == false);
                }
            }
        }

        /// <summary>
        /// Asynchronously waits for the specified client to log in using player name and desired AI name.
        /// </summary>
        /// <param name="connection">The client connection.</param>
        private async void WaitForLogin(ClientConnection connection)
        {
            while (true)
            {
                if (!connection.IsConnected)
                    return;

                var clientMessage = await connection.ReceiveClientMessageAsync();
                if (!(clientMessage is LoginMessage))
                {
                    Console.WriteLine("Client has sent invalid message for connecting.");
                    await connection.TrySendAsync("FAIL invalid message format.");
                }
                else
                {
                    if (await AuthenticationHandler((LoginMessage)clientMessage, connection))
                    {
                        await connection.TrySendAsync("CONNECTED");
                        connection.IsActive = true;
                        lock (ActiveConnections)
                        {
                            ActiveConnections.Add(connection);
                        }
                        Console.WriteLine($"Player {connection.PlayerName} with AI {connection.AiName} has log on.");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the login message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        /// <returns><c>true</c> if the client has log on successfully; otherwise <c>false</c></returns>
        private Task<bool> DefaultAuthenticate(LoginMessage message, ClientConnection connection)
        {
            return Task.FromResult(true);
        }

    }
}
