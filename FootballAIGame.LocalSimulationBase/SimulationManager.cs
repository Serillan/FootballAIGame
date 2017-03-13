using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FootballAIGame.LocalSimulationBase.Models;
using FootballAIGame.MatchSimulation;
using FootballAIGame.MatchSimulation.Messages;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.LocalSimulationBase
{
    public class SimulationManager
    {
        private ISet<string> ConnectedAiNames { get; set; }

        public List<MatchSimulator> RunningSimulations { get; set; } = new List<MatchSimulator>();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SimulationManager Instance => _instance ?? (_instance = new SimulationManager());

        private static SimulationManager _instance; // singleton instance

        private SimulationManager()
        {
            Initialize();
        }

        public async Task<Match> Simulate(string ai1, string ai2)
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

        public async Task StartSimulating()
        {
            await ConnectionManager.Instance.StartListeningAsync();
        }

        private void Initialize()
        {
            ConnectedAiNames = new HashSet<string>();

            SetSimulationHandlers();

            ConnectionManager.Instance.CheckConnectionsInterval = 500; // lower interval
        }

        private void SetSimulationHandlers()
        {
            ConnectionManager.Instance.AuthenticationHandler = ProcessLoginMessageAsync;
            ConnectionManager.Instance.PlayerDisconectedHandler += PlayerDisconectedHandler;
        }

        private async Task PlayerDisconectedHandler(ClientConnection connection)
        {
            await Task.Yield();

            ConnectedAiNames.Remove(connection.AiName);
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
    }
}
