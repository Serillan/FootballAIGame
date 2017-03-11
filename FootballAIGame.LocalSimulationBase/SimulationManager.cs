using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGame.LocalSimulationBase.Models;
using FootballAIGame.MatchSimulation;
using FootballAIGame.MatchSimulation.Messages;

namespace FootballAIGame.LocalSimulationBase
{
    public class SimulationManager
    {
        private ISet<string> ConnectedAiNames { get; set; }

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

        public async Task<Match> Simulate(string ai, string ai2)
        {
            await Task.Delay(100000);

            return null;



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
