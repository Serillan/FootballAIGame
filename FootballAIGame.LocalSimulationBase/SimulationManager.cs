using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.LocalSimulationBase.Models;
using FootballAIGame.MatchSimulation;
using FootballAIGame.MatchSimulation.Messages;

namespace FootballAIGame.LocalSimulationBase
{
    /// <summary>
    /// Provides functionality to manage match simulations and AIs' connections for local simulators. Serves as the
    /// bridge between MatchSimulation library and local simulators. Implemented as singleton.
    /// </summary>
    public class SimulationManager
    {
        /// <summary>
        /// Gets or sets the set of connected AIs.
        /// </summary>
        /// <value>
        /// The <see cref="ISet{T}"/> containing the connected AIs' names.
        /// </value>
        private ISet<string> ConnectedAiNames { get; set; }

        /// <summary>
        /// Gets or sets the running simulations.
        /// </summary>
        /// <value>
        /// The <see cref="IList{T}"/> containing the running simulations.
        /// </value>
        private IList<MatchSimulator> RunningSimulations { get; set; } = new List<MatchSimulator>();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SimulationManager Instance => _instance ?? (_instance = new SimulationManager());

        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static SimulationManager _instance; // singleton instance

        /// <summary>
        /// Prevents a default instance of the <see cref="SimulationManager"/> class from being created.
        /// </summary>
        private SimulationManager()
        {
            Initialize();
        }

        /// <summary>
        /// Simulates the match between the specified AIs asynchronously.
        /// </summary>
        /// <param name="ai1">The first AI name.</param>
        /// <param name="ai2">The second AI name.</param>
        /// <returns>The task that represents the asynchronous simulate operation. 
        /// The value of the task's result is the <see cref="Match"/> that represents the simulated match with
        /// all the information about the simulation set.</returns>
        /// <exception cref="InvalidOperationException">
        /// The specified AIs are the same or at least one of the specified AIs is already in the match.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// At least one of the specified AIs in not logged in.
        /// </exception>
        public async Task<Match> SimulateAsync(string ai1, string ai2)
        {
            if (ai1 == ai2)
                throw new InvalidOperationException("AIs must be different.");

            ClientConnection connection1;
            ClientConnection connection2;

            var activeConnection = ConnectionManager.Instance.ActiveConnections;

            lock (activeConnection)
            {
                connection1 = activeConnection.FirstOrDefault(c => c.AiName == ai1);
                connection2 = activeConnection.FirstOrDefault(c => c.AiName == ai2);

                if (connection1 == null) throw new ArgumentException($"{ai1} is not active.");
                if (connection2 == null) throw new ArgumentException($"{ai2} is not active");

                if (connection1.IsInMatch)
                    throw new InvalidOperationException($"{ai1} is already in a match");
                if (connection2.IsInMatch)
                    throw new InvalidOperationException($"{ai2} is already in a match");

                connection1.IsInMatch = true;
                connection2.IsInMatch = true;
            }

            var simulation = new MatchSimulator(connection1, connection2);

            lock (RunningSimulations)
            {
                RunningSimulations.Add(simulation);
            }

            await simulation.SimulateMatchAsync();

            lock (RunningSimulations)
            {
                RunningSimulations.Remove(simulation);
            }

            connection1.IsInMatch = false;
            connection2.IsInMatch = false;

            var match = new Match()
            {
                MatchInfo = simulation.MatchInfo,
                Ai1Name = ai1,
                Ai2Name = ai2
            };

            return match;
        }

        /// <summary>
        /// Starts accepting AIs connections asynchronously.
        /// </summary>
        /// <returns>The task that represents the asynchronous operation of accepting connections.</returns>
        public async Task StartAcceptingConnectionsAsync()
        {
            await ConnectionManager.Instance.StartListeningAsync();
        }

        /// <summary>
        /// Tries to get the simulation step of the currently simulated match between the specified AIs.
        /// </summary>
        /// <param name="ai1">The name of the first AI.</param>
        /// <param name="ai2">The name of the second AI.</param>
        /// <param name="step">The simulation step of the currently simulated match between the specified AIs.</param>
        /// <returns><c>true</c> if the specified AIs are in the currently simulated match and the
        /// simulation step was set accordingly; otherwise, <c>false</c></returns>
        public bool TryGetSimulationStep(string ai1, string ai2, out int step)
        {
            var simulation = GetRunningSimulation(ai1, ai2);

            if (simulation == null)
            {
                step = 0;
                return false;
            }

            step = simulation.CurrentStep;
            return true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            ConnectedAiNames = new HashSet<string>();

            SetSimulationHandlers();

            ConnectionManager.Instance.CheckConnectionsInterval = 500; // lower interval
        }

        /// <summary>
        /// Sets the simulation handlers. Sets <see cref="ConnectionManager"/> handlers for client's authentication and
        /// disconnection.
        /// </summary>
        private void SetSimulationHandlers()
        {
            ConnectionManager.Instance.AuthenticationHandler = ProcessLoginMessageAsync;
            ConnectionManager.Instance.ActiveClientDisconnectedHandler += HandlePlayerDisconnectionAsync;
        }

        /// <summary>
        /// Handles the player disconnection asynchronously.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The task that represents the asynchronous operation.</returns>
        private async Task HandlePlayerDisconnectionAsync(ClientConnection connection)
        {
            await Task.Yield();

            lock (ConnectedAiNames)
            {
                ConnectedAiNames.Remove(connection.AiName);
            }
        }

        /// <summary>
        /// Processes the login message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection.</param>
        /// <returns><c>true</c> if the client has log on successfully; otherwise <c>false</c></returns>
        private async Task<bool> ProcessLoginMessageAsync(LoginMessage message, ClientConnection connection)
        {
            await Task.Yield();

            // player name is ignored! (AiName is used instead for differentiation)
            message.PlayerName = message.AiName;

            bool isNameUnused;

            lock (ConnectedAiNames)
            {
                isNameUnused = !ConnectedAiNames.Contains(message.AiName);
                if (isNameUnused)
                    ConnectedAiNames.Add(message.AiName);
            }

            if (!isNameUnused)
                await connection.TrySendAsync("AI name is already being used.");

            return isNameUnused;
        }

        /// <summary>
        /// Gets the running simulation between the specified AIs.
        /// </summary>
        /// <param name="ai1">The name of the first AI.</param>
        /// <param name="ai2">The name of the second AI.</param>
        /// <returns><c>null</c> if there is not a running simulation between the specified AIs; otherwise 
        /// the <see cref="MatchSimulator"/> simulating the match between the specified AIs.</returns>
        private MatchSimulator GetRunningSimulation(string ai1, string ai2)
        {
            lock (RunningSimulations)
            {
                return RunningSimulations.FirstOrDefault(s =>
                    (s.AI1Communicator.PlayerName == ai1 && s.AI2Communicator.PlayerName == ai2) ||
                    (s.AI1Communicator.PlayerName == ai2 && s.AI2Communicator.PlayerName == ai1));
            }
        }

        /// <summary>
        /// Stops the simulation between the specified AIs.
        /// </summary>
        /// <param name="ai1">The name of the first AI.</param>
        /// <param name="ai2">The name of the second AI.</param>
        public void StopSimulation(string ai1, string ai2)
        {
            var simulation = GetRunningSimulation(ai1, ai2);
            if (simulation == null)
                return;

            simulation.AI1CancelRequested = true;
        }
    }
}
