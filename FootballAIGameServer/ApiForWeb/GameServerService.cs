using System;
using System.Collections.Generic;
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
            try
            {
                var manager = ConnectionManager.Instance;
                lock (manager.ActiveConnections)
                {
                    var connection = manager.ActiveConnections
                        .FirstOrDefault(c => c.PlayerName == userName && c.AiName == ai);
                    if (connection == null)
                        return "AI is no longer active.";

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
                            player1.PlayerState = PlayerState.PlayingMatch;
                            player2.PlayerState = PlayerState.PlayingMatch;

                            context.SaveChanges();
                        }

                        StartGame(connection.PlayerName, connection.AiName,
                            otherPlayerConnection.PlayerName, otherPlayerConnection.AiName);
                    }

                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("wants to play exception");
                Console.WriteLine(ex.Message);
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
            var manager = ConnectionManager.Instance;
            lock (manager.ActiveConnections)
            {
                var connection1 = manager.ActiveConnections
                    .FirstOrDefault(c => c.PlayerName == userName1 && c.AiName == ai1);
                var connection2 = manager.ActiveConnections
                    .FirstOrDefault(c => c.PlayerName == userName2 && c.AiName == ai2);

                if (connection1 == null || connection2 == null)
                    return "AI is no longer active.";

                MatchSimulator matchSimulator = new MatchSimulator(connection1, connection2);
                matchSimulator.SimulateMatch();

                return "ok";
            }
        }

        /// <summary>
        /// Cancels the game match in which a given player is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public void CancelMatch(string playerName)
        {
            foreach (var runningSimulation in MatchSimulator.RunningSimulations)
            {
                if (runningSimulation.Player1AiConnection.PlayerName == playerName)
                    runningSimulation.Player1CancelRequested = true;
                if (runningSimulation.Player2AiConnection.PlayerName == playerName)
                    runningSimulation.Player2CancelRequested = true;
            }
        }

        /// <summary>
        /// Removes player from the random match queue.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        public void CancelLooking(string playerName)
        {
            ConnectionManager.Instance.WantsToPlayConnections.RemoveAll(p => p.PlayerName == playerName);
        }
    }
}
