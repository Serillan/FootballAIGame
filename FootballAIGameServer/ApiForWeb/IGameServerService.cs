using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FootballAIGameServer.ApiForWeb
{
    /// <summary>
    /// WCF service contract for application web server.
    /// </summary>
    [ServiceContract]
    public interface IGameServerService
    {
        /// <summary>
        /// Add player with a given name and AI to the queue for random match.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="ai">The AI name.</param>
        /// <returns>"ok" if operation was successful; otherwise, error message</returns>
        [OperationContract]
        string WantsToPlay(string userName, string ai);

        /// <summary>
        /// Starts the game between the given players AIs.
        /// </summary>
        /// <param name="userName1">The player1 name.</param>
        /// <param name="ai1">The player1 AI name.</param>
        /// <param name="userName2">The player2 name.</param>
        /// <param name="ai2">The player2 AI name.</param>
        /// <returns>"ok" if operation was successful; otherwise, error message</returns>
        [OperationContract]
        string StartGame(string userName1, string ai1, string userName2, string ai2);

        /// <summary>
        /// Cancels the game match in which a given player is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        [OperationContract]
        void CancelMatch(string playerName);

        /// <summary>
        /// Player will leave a running tournament in which he currently is.
        /// If there is not such tournament, then it does nothing.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        [OperationContract]
        void LeaveRunningTournament(string playerName);

        /// <summary>
        /// Plans the tournament.
        /// </summary>
        /// <param name="tournamentId">The tournament identifier.</param>
        [OperationContract]
        void PlanTournament(int tournamentId);

        /// <summary>
        /// Removes player from the random match queue.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        [OperationContract]
        void CancelLooking(string playerName);

        /// <summary>
        /// Gets the current simulation step of the match in which the specified player currently is.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        [OperationContract]
        int GetCurrentMatchStep(string playerName);
    }
}
