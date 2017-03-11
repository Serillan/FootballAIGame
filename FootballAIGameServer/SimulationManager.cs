using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulator;
using FootballAIGame.MatchSimulator.Messages;
using FootballAIGame.Server.Models;

namespace FootballAIGame.Server
{
    /// <summary>
    /// Responsible for managing <see cref="MatchSimulator"/> instances.
    /// It provides methods for starting new simulations and accessing currently
    /// running simulations.
    /// </summary>
    public class SimulationManager
    {
        /// <summary>
        /// Gets or sets the list of currently running simulations.
        /// </summary>
        /// <value>
        /// The list of currently running simulations.
        /// </value>
        public List<MatchSimulator.MatchSimulator> RunningSimulations { get; set; } = new List<MatchSimulator.MatchSimulator>();

        /// <summary>
        /// Gets or sets the wants to play connections.
        /// </summary>
        /// <value>
        /// The wants to play connections.
        /// </value>
        public List<ClientConnection> WantsToPlayConnections { get; set; } = new List<ClientConnection>();

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

        public async Task StartSimulatingAsync()
        {
            TournamentSimulator.PlanUnstartedTournaments();
            Console.WriteLine("Tournaments planned.");

            var listening = ConnectionManager.Instance.StartListeningAsync();
            Console.WriteLine("Listening has started.");

            await listening;
        }

        /// <summary>
        /// Cancels the game match in which a given player is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void CancelMatch(string playerName)
        {
            lock (RunningSimulations)
            {
                foreach (var runningSimulation in RunningSimulations)
                {
                    if (runningSimulation.Player1AiConnection.PlayerName == playerName)
                        runningSimulation.Player1CancelRequested = true;
                    if (runningSimulation.Player2AiConnection.PlayerName == playerName)
                        runningSimulation.Player2CancelRequested = true;
                }
            }
        }

        /// <summary>
        /// Starts the game between the given players AIs.
        /// </summary>
        /// <param name="userName1">The player1 name.</param>
        /// <param name="ai1">The player1 AI name.</param>
        /// <param name="userName2">The player2 name.</param>
        /// <param name="ai2">The player2 AI name.</param>
        /// <param name="tournamentId">The tournament Id. (optional)</param>
        /// <returns>
        /// "ok" if operation was successful; otherwise, error message
        /// </returns>
        public string StartMatch(string userName1, string ai1, string userName2, string ai2, int? tournamentId = null)
        {
            var manager = ConnectionManager.Instance;
            lock (manager.ActiveConnections)
            {
                var connection1 = manager.ActiveConnections
                    .FirstOrDefault(c => c.PlayerName == userName1 && c.AiName == ai1);
                var connection2 = manager.ActiveConnections
                    .FirstOrDefault(c => c.PlayerName == userName2 && c.AiName == ai2);

                if (connection1 == null || connection2 == null)
                    return "AI is no longer active.";

                if (connection1.IsInMatch || connection2.IsInMatch)
                    return "Player is already in a match";

                if (userName1 == userName2)
                    Console.WriteLine("User cannot challenge himself.");

                var matchSimulator = new MatchSimulator.MatchSimulator(connection1, connection2);

                lock (RunningSimulations)
                {
                    RunningSimulations.Add(matchSimulator);
                }

                var startTime = DateTime.Now;

                matchSimulator.SimulateMatchAsync().ContinueWith((task => OnSimulationEndAsync(startTime, matchSimulator, tournamentId)));

                return "ok";
            }
        }

        /// <summary>
        /// Gets the match simulator that is simulating match in which the specified player is.
        /// </summary>
        /// <param name="userName">Name of the player.</param>
        /// <returns></returns>
        public MatchSimulator.MatchSimulator GetMatchSimulator(string userName)
        {
            return RunningSimulations
                .FirstOrDefault(m => m.Player1AiConnection.PlayerName == userName ||
                                     m.Player2AiConnection.PlayerName == userName);
        }

        private void Initialize()
        {
            ResetPlayers();

            SetSimulationHandlers();
        }

        private void ResetPlayers()
        {
            // reset player states on the start
            using (var context = new ApplicationDbContext())
            {
                var players = context.Players.ToList();
                foreach (var player in players)
                {
                    player.PlayerState = PlayerState.Idle;
                    player.ActiveAis = null;
                    player.SelectedAi = null;
                }
                context.SaveChanges();
            }
        }

        private void SetSimulationHandlers()
        {
            ConnectionManager.Instance.AuthenticationHandler = ProcessLoginMessageAsync;
            ConnectionManager.Instance.PlayerDisconectedHandler = ProcessClientDisconnectionAsync;
        }

        private async Task OnSimulationEndAsync(DateTime startTime, MatchSimulator.MatchSimulator simulator, int? tournamentId)
        {
            var matchInfo = simulator.MatchInfo;

            var player1AiConnection = simulator.Player1AiConnection as ClientConnection;
            var player2AiConnection = simulator.Player2AiConnection as ClientConnection;
            Debug.Assert(player1AiConnection != null, "player1AiConnection != null");
            Debug.Assert(player2AiConnection != null, "player2AiConnection != null");


            using (var context = new ApplicationDbContext())
            {
                var player1Retrieve = context.Players.SingleAsync(p => p.Name == player1AiConnection.PlayerName);
                var player2 = await context.Players.SingleAsync(p => p.Name == player2AiConnection.PlayerName);
                var player1 = await player1Retrieve;

                player1.PlayerState = player1.PlayerState == PlayerState.PlayingTournamentPlaying ?
                    PlayerState.PlayingTournamentWaiting : PlayerState.Idle;
                player2.PlayerState = player2.PlayerState == PlayerState.PlayingTournamentPlaying ?
                    PlayerState.PlayingTournamentWaiting : PlayerState.Idle;

                var match = new Match
                {
                    Time = startTime,
                    Player1 = player1,
                    Player2 = player2,
                    Player1Ai = player1AiConnection.AiName,
                    Player2Ai = player2AiConnection.AiName,
                    Score = $"{simulator.MatchInfo.Goals1}:{simulator.MatchInfo.Goals2}",
                    Winner = matchInfo.Goals1 > matchInfo.Goals2 ? 1 : matchInfo.Goals1 < matchInfo.Goals2 ? 2 : 0,
                    Goals = matchInfo.Goals,
                    TournamentId = tournamentId
                };


                if (matchInfo.Winner == player1AiConnection.PlayerName)
                    player1.WonGames++;
                else if (matchInfo.Winner == player2AiConnection.PlayerName)
                    player2.WonGames++;

                // convert match data to byte array
                var matchByteData = new byte[matchInfo.MatchData.Count * 4];
                Buffer.BlockCopy(matchInfo.MatchData.ToArray(), 0, matchByteData, 0, matchInfo.MatchData.Count * 4);
                match.MatchData = matchByteData;

                context.Matches.Add(match);

                await context.SaveChangesAsync();

                RunningSimulations.Remove(simulator);

                player1AiConnection.IsInMatch = false;
                player2AiConnection.IsInMatch = false;
            }
        }

        private async Task ProcessClientDisconnectionAsync(ClientConnection connection)
        {
            using (var context = new ApplicationDbContext())
            {
                var player =
                    await context.Players.SingleOrDefaultAsync(p => p.Name == connection.PlayerName);

                if (player != null)
                {
                    var newAis = player.ActiveAis.Split(';').Where(s => s != connection.AiName);
                    player.ActiveAis = string.Join(";", newAis);

                    lock (WantsToPlayConnections)
                    {
                        WantsToPlayConnections.Remove(connection);
                    }

                    if (player.SelectedAi == connection.AiName)
                    {
                        player.SelectedAi = "";
                        player.PlayerState = PlayerState.Idle; // todo error message
                    }

                    if (player.ActiveAis == "")
                        player.ActiveAis = null;
                    connection.Dispose();
                }
                await context.SaveChangesAsync();
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
            using (var context = new ApplicationDbContext())
            {
                var player = await context.Players
                    .Include(u => u.User)
                    .FirstOrDefaultAsync(p => p.Name == message.PlayerName);
                if (player == null)
                {
                    await connection.TrySendAsync("Invalid player name");
                    return false;
                }
                if (player.ActiveAis == null)
                    player.ActiveAis = message.AiName;
                else
                {
                    if (player.ActiveAis.Split(';').Contains(message.AiName))
                    {
                        await connection.TrySendAsync("AI name is already being used.");
                        return false;
                    }
                    player.ActiveAis += ";" + message.AiName;
                }
                connection.AiName = message.AiName;
                connection.PlayerName = message.PlayerName;
                await context.SaveChangesAsync();
            }

            return true;
        }


    }
}
