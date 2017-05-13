using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.Messages;

namespace FootballAIGame.MatchSimulation
{
    /// <summary>
    /// Represents the method that is used to handle the client's disconnection asynchronously.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns>The task that represents the asynchronous handle operation.</returns>
    public delegate Task ClientDisconnectedHandler(ClientConnection connection);

    /// <summary>
    /// Represents the method that is used to handle the new logged in client asynchronously.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <returns>The task that represents the asynchronous handle operation.</returns>
    public delegate Task ClientLoggedInHandler(ClientConnection connection);

    /// <summary>
    /// Represents the method that is used to authenticate the client asynchronously.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The task that represents the asynchronous handle operation.
    /// The value of the task's result is null if the client has successfully authenticated;
    /// otherwise, an error message.</returns>
    public delegate Task<string> AuthenticationHandler(LoginMessage message);

    /// <summary>
    /// Responsible for managing client connections.
    /// Provides the method to start listening for new connections and public properties to access these connections. 
    /// Implemented as singleton. 
    /// </summary>
    public class ConnectionManager
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static ConnectionManager _instance;

        /// <summary>
        /// Gets or sets a value indicating whether the information about the new connections and disconnections
        /// should be written to the output.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is verbose; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerbose { get; set; }

        /// <summary>
        /// Gets or sets the interval in milliseconds in which the clients are checked
        /// whether they are still connected.
        /// </summary>
        /// <value>
        /// The check connections interval.
        /// </value>
        public int CheckConnectionsInterval { get; set; } = 5000; // [ms]

        /// <summary>
        /// Gets or sets the <see cref="List{T}"/> of connected <see cref="ClientConnection"/>s.
        /// </summary>
        /// <value>
        /// The <see cref="List{T}"/> of connected <see cref="ClientConnection"/>.
        /// </value>
        public List<ClientConnection> Connections { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="List{T}"/> of active <see cref="ClientConnection"/>s. 
        /// Connection becomes active when the client successfully logs in.
        /// </summary>
        /// <value>
        /// The <see cref="List{T}"/> of active <see cref="ClientConnection"/>s.
        /// </value>
        public List<ClientConnection> ActiveConnections { get; set; }

        /// <summary>
        /// Gets or sets the authentication handler that is used for authenticating
        /// a <see cref="ClientConnection"/>.
        /// </summary>
        /// <value>
        /// The <see cref="AuthenticationHandler"/>.
        /// </value>
        public AuthenticationHandler AuthenticationHandler { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ActiveClientDisconnectedHandler"/> that is called when a 
        /// logged in <see cref="ClientConnection"/> disconnects.
        /// </summary>
        /// <value>
        /// The <see cref="MatchSimulation.ClientDisconnectedHandler"/>.
        /// </value>
        public ClientDisconnectedHandler ActiveClientDisconnectedHandler { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MatchSimulation.ClientLoggedInHandler"/> that is called when
        /// the client successfully logs in.
        /// </summary>
        /// <value>
        /// The <see cref="ClientLoggedInHandler"/>.
        /// </value>
        public ClientLoggedInHandler ClientLoggedInHandler { get; set; }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ConnectionManager Instance => _instance ?? (_instance = new ConnectionManager());



        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class. <para/>
        /// Prevents a default instance of the <see cref="ConnectionManager"/> class from being created.
        /// </summary>
        private ConnectionManager()
        {
            Connections = new List<ClientConnection>();
            ActiveConnections = new List<ClientConnection>();
            AuthenticationHandler = DoDefaultAuthenticationAsync;
        }

        /// <summary>
        /// Starts the listening for new AI clients. When the client connects, he is added to
        /// the <see cref="Connections" /> and he remains there while the connection is opened.
        /// Returns if the listening couldn't start (also writes the error message to the
        /// standard error output).
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>
        /// The task that represents the asynchronous listening operation.
        /// </returns>
        public async Task StartListeningAsync(int port)
        {
            TcpListener listener;

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
            }
            catch (SocketException)
            {
                Console.Error.WriteLine($"Error: The port {port} is already being used by another process. " +
                                        $"You might already have another simulator on.");
                return;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.Error.WriteLine($"Error: The specified port {port} is out of range of valid ports.");
                return;
            }

            if (IsVerbose)
                Console.WriteLine($"Listening has started on port {port}.");

            var checkingConnectionsTask = StartCheckingConnectionsAsync();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                var connection = new ClientConnection(client);
                lock (Connections)
                {
                    Connections.Add(connection);
                }

                if (IsVerbose)
                    Console.WriteLine("New client connection established.");

                // fire and forget
                var loginTask = HandleClientLoggingInAsync(connection); 
            }
        }

        /// <summary>
        /// Starts checking that clients from <see cref="Connections"/> are still connected 
        /// and keep sending keep alive packets to clients that are not currently in a match. <para />
        /// The interval of doing it is specified by <see cref="CheckConnectionsInterval"/>.
        /// </summary>
        public async Task StartCheckingConnectionsAsync()
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
                            if (clientConnection.IsLoggedIn && IsVerbose)
                                Console.WriteLine($"Player {clientConnection.PlayerName} with AI " +
                                              $"{clientConnection.AiName} has disconnected.");

                            clientConnection.IsLoggedIn = false;

                            toBeRemovedConnections.Add(clientConnection);

                        }
                        else if (!clientConnection.IsInMatch && clientConnection.IsLoggedIn)
                        {
                            runningTasks.Add(clientConnection.TrySendAsync("keepalive"));
                        }
                    }

                    Connections.RemoveAll(c => toBeRemovedConnections.Contains(c));

                }


                lock (ActiveConnections)
                {
                    ActiveConnections.RemoveAll(c => c.IsLoggedIn == false);
                }

                foreach (var connection in toBeRemovedConnections)
                {
                    if (ActiveClientDisconnectedHandler != null)
                        runningTasks.Add(ActiveClientDisconnectedHandler(connection));

                }

                await Task.WhenAll(runningTasks);

                toBeRemovedConnections.ForEach(c => c.Dispose());
            }
        }

        /// <summary>
        /// Handles client's logging in asynchronously.
        /// </summary>
        /// <param name="connection">The client's connection.</param>
        /// <returns>The task that represents the asynchronous logging in operation.</returns>
        private async Task HandleClientLoggingInAsync(ClientConnection connection)
        {
            while (true)
            {
                if (!connection.IsConnected)
                    return;

                var clientMessage = await connection.ReceiveClientMessageAsync();

                if (clientMessage == null) // connection has dropped
                {
                    if (IsVerbose)
                        Console.WriteLine("Unauthenticated client has disconnected.");
                    break; 
                }
                else if (!(clientMessage is LoginMessage))
                {
                    if (IsVerbose)
                        Console.WriteLine("Client has sent invalid message for connecting.");
                    await connection.TrySendAsync("Invalid message format.");
                }
                else
                {
                    var loginMessage = clientMessage as LoginMessage;

                    string errorMessage;

                    if ((errorMessage = await AuthenticationHandler(loginMessage)) == null)
                    {
                        connection.AiName = loginMessage.AIName;
                        connection.PlayerName = loginMessage.PlayerName;

                        await connection.TrySendAsync("CONNECTED");
                        connection.IsLoggedIn = true;
                        lock (ActiveConnections)
                        {
                            ActiveConnections.Add(connection);
                        }

                        if (IsVerbose)
                            Console.WriteLine($"Player {connection.PlayerName} with AI {connection.AiName} has logged in.");
                        if (ClientLoggedInHandler != null)
                            await ClientLoggedInHandler(connection);
                        break;
                    }
                    else
                    {
                        if (IsVerbose)
                            Console.WriteLine("Client authentication error: " + errorMessage);
                        await connection.TrySendAsync(errorMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Does default asynchronous client authentication. Its the default value of <see cref="AuthenticationHandler"/>.
        /// </summary>
        /// <param name="message">The client's login message.</param>
        /// <returns>The task that represents the asynchronous logging in operation.
        /// The value of the task's result is null if the client has successfully authenticated;
        /// otherwise, an error message.</returns>
        private Task<string> DoDefaultAuthenticationAsync(LoginMessage message)
        {
            return Task.FromResult((string)null);
        }

    }
}
