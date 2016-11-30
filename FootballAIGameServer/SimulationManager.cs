using System;
using System.Collections.Generic;
using System.Linq;

namespace FootballAIGameServer
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
        public static List<MatchSimulator> RunningSimulations { get; set; } = new List<MatchSimulator>();

        /// <summary>
        /// Cancels the game match in which a given player is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        public static void CancelMatch(string playerName)
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
        public static string StartMatch(string userName1, string ai1, string userName2, string ai2, int? tournamentId = null)
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

                var matchSimulator = new MatchSimulator(connection1, connection2, tournamentId);

                lock (RunningSimulations)
                {
                    RunningSimulations.Add(matchSimulator);
                }

                matchSimulator.SimulateMatch();

                return "ok";
            }
        }

        /// <summary>
        /// Gets the match simulator that is simulating match in which the specified player is.
        /// </summary>
        /// <param name="userName">Name of the player.</param>
        /// <returns></returns>
        public static MatchSimulator GetMatchSimulator(string userName)
        {
            return RunningSimulations
                .FirstOrDefault(m => m.Player1AiConnection.PlayerName == userName ||
                                     m.Player2AiConnection.PlayerName == userName);
        }
    }
}
