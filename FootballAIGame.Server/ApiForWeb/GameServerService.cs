
namespace FootballAIGame.Server.ApiForWeb
{
    /// <summary>
    /// Represents the WCF service for application web server.
    /// </summary>
    /// <seealso cref="FootballAIGame.Server.ApiForWeb.IGameServerService" />
    class GameServerService : IGameServerService
    {
        /// <summary>
        /// Adds player with the specified AI to the queue for a random match.
        /// </summary>
        /// <param name="userName">Name of the player.</param>
        /// <param name="ai">The AI's name.</param>
        /// <returns>
        /// "ok" if the operation was successful; otherwise, an error message.
        /// </returns>
        public string AddToLookingForRandomOpponent(string userName, string ai)
        {
            return SimulationManager.Instance.AddToWantsToPlayConnections(userName, ai);
        }

        /// <summary>
        /// Removes player from the random match queue.
        /// </summary>
        /// <param name="playerName">The player's name.</param>
        public void RemoveFromLookingForRandomOpponent(string playerName)
        {
            SimulationManager.Instance.RemoveFromWantsToPlayConnections(playerName);
        }

        /// <summary>
        /// Starts the match between specified players' AIs.
        /// </summary>
        /// <param name="userName1">The player1's name.</param>
        /// <param name="ai1">The player1's AI name.</param>
        /// <param name="userName2">The player2's name.</param>
        /// <param name="ai2">The player2's AI name.</param>
        /// <returns>
        /// "ok" if the operation was successful; otherwise, an error message.
        /// </returns>
        public string StartMatch(string userName1, string ai1, string userName2, string ai2)
        {
            return SimulationManager.Instance.StartMatch(userName1, ai1, userName2, ai2);
        }

        /// <summary>
        /// Cancels the match in which the specified player is.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        public void CancelMatch(string playerName)
        {
            SimulationManager.Instance.CancelMatch(playerName);
        }

        /// <summary>
        /// Gets the current simulation step of the match in which the specified player currently is.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <returns>
        /// The match step of the match in which the specified player is or 1500 if the player
        /// isn't in any running match.
        /// </returns>        
        public int GetPlayerCurrentMatchStep(string playerName)
        {
            return SimulationManager.Instance.GetMatchStep(playerName);
        }

        /// <summary>
        /// Plans the tournament.
        /// </summary>
        /// <param name="tournamentId">The tournament's identifier.</param>
        public void PlanTournament(int tournamentId)
        {
            TournamentManager.PlanTournament(tournamentId);
        }

        /// <summary>
        /// Remove player from a running tournament in which he currently is.
        /// If there is not such tournament, then it does nothing.
        /// </summary>
        /// <param name="playerName">The player's name.</param>
        public void RemoveFromRunningTournament(string playerName)
        {
            // cancel tournament match if player is in it
            SimulationManager.Instance.CancelMatch(playerName);
            TournamentManager.Instance.RemoveFromRunningTournament(playerName);
        }

    }
}
