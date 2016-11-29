using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FootballAIGameServer.Models;

namespace FootballAIGameServer.ApiForWeb
{
    /// <summary>
    /// WCF service for application web server.
    /// </summary>
    /// <seealso cref="FootballAIGameServer.ApiForWeb.IGameServerService" />
    public class GameServerService : IGameServerService
    {
        /// <summary>
        /// Add player with a given name and AI to the queue for random match.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="ai">The AI name.</param>
        /// <returns>
        /// "ok" if operation was successful; otherwise, error message
        /// </returns>
        public string WantsToPlay(string userName, string ai)
        {
            var manager = ConnectionManager.Instance;

            ClientConnection connection;
            lock (manager.ActiveConnections)
            {
                connection = manager.ActiveConnections
                    .FirstOrDefault(c => c.PlayerName == userName && c.AiName == ai);
            }

            if (connection == null)
                return "AI is no longer active.";

            lock (manager.WantsToPlayConnections)
            {
                if (manager.WantsToPlayConnections.Count == 0)
                {
                    manager.WantsToPlayConnections.Add(connection);
                }
                else // start match
                {
                    var otherPlayerConnection = manager.WantsToPlayConnections[0];
                    manager.WantsToPlayConnections.Remove(otherPlayerConnection);

                    using (var context = new ApplicationDbContext())
                    {
                        var player1 = context.Players.FirstOrDefault(p => p.Name == connection.PlayerName);
                        var player2 = context.Players.FirstOrDefault(p => p.Name == otherPlayerConnection.PlayerName);

                        if (otherPlayerConnection == connection)
                            return "Player is already looking for opponent.";

                        player1.PlayerState = PlayerState.PlayingMatch;
                        player2.PlayerState = PlayerState.PlayingMatch;

                        context.SaveChanges();
                    }

                    StartGame(connection.PlayerName, connection.AiName,
                        otherPlayerConnection.PlayerName, otherPlayerConnection.AiName);
                }
            }
            return "ok";
        }

        /// <summary>
        /// Starts the game between the given players AIs.
        /// </summary>
        /// <param name="userName1">The player1 name.</param>
        /// <param name="ai1">The player1 AI name.</param>
        /// <param name="userName2">The player2 name.</param>
        /// <param name="ai2">The player2 AI name.</param>
        /// <returns>
        /// "ok" if operation was successful; otherwise, error message
        /// </returns>
        public string StartGame(string userName1, string ai1, string userName2, string ai2)
        {
            return SimulationManager.StartMatch(userName1, ai1, userName2, ai2);
        }

        /// <summary>
        /// Cancels the game match in which a given player is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void CancelMatch(string playerName)
        {
            SimulationManager.CancelMatch(playerName);
        }

        /// <summary>
        /// Player will leave a running tournament in which he currently is.
        /// If there is not such tournament, then it does nothing.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        public void LeaveRunningTournament(string playerName)
        {
            // cancel tournament match if player is in it
            SimulationManager.CancelMatch(playerName);
            TournamentSimulator.LeaveRunningTournament(playerName);
        }

        /// <summary>
        /// Removes player from the random match queue.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        public void CancelLooking(string playerName)
        {
            lock (ConnectionManager.Instance.WantsToPlayConnections)
            {
                ConnectionManager.Instance.WantsToPlayConnections.RemoveAll(p => p.PlayerName == playerName);
            }
        }

        /// <summary>
        /// Gets the current simulation step of the match in which the specified player currently is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns></returns>
        public int GetCurrentMatchStep(string playerName)
        {
            var match = SimulationManager.RunningSimulations
                .FirstOrDefault(m => m.Player1AiConnection.PlayerName == playerName ||
                                     m.Player2AiConnection.PlayerName == playerName);
            return match?.CurrentStep ?? 1500;
        }
    }
}
