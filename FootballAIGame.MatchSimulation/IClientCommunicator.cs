using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.Messages;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.MatchSimulation
{
    /// <summary>
    /// Provides a mechanism for communicating with the client.
    /// </summary>
    public interface IClientCommunicator
    {
        /// <summary>
        /// Gets or sets the name of the player with which the client has log on.
        /// </summary>
        /// <value>
        /// The name of the player.
        /// </value>
        string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is logged in.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the client is logged in; otherwise, <c>false</c>.
        /// </value>
        bool IsLoggedIn { get; set; }

        /// <summary>
        /// Sends the specified message to the client asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task that represents the asynchronous send operation.</returns>
        Task SendAsync(string message);

        /// <summary>
        /// Sends the specified <see cref="GameState" /> to the client asynchronously.
        /// </summary>
        /// <param name="gameState">The state of the game.</param>
        /// <param name="team">The <see cref="Team" /> belonging to this connection.</param>
        /// <returns>The task that represents the asynchronous send operation. </returns>
        Task SendAsync(GameState gameState, Team team);

        /// <summary>
        /// Tries to send the message to the client asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The task that represents the asynchronous send operation. 
        /// The value of the task's result is <c>true</c> if the sending was successful; otherwise <c>false</c>.</returns>
        Task<bool> TrySendAsync(string message);

        /// <summary>
        /// Tries to send the specified game state to the client asynchronously.
        /// </summary>
        /// <param name="gameState">The state of the game.</param>
        /// <param name="team">The <see cref="Team"/> belonging to this connection.</param>
        /// <returns>The task that represents the asynchronous send operation. 
        /// The value of the task's result is <c>true</c> if the sending was successful; otherwise <c>false</c>.</returns>
        Task<bool> TrySendAsync(GameState gameState, Team team);

        /// <summary>
        /// Receives a client message asynchronously.
        /// The task's result is null if the connection is dropped.
        /// </summary>
        /// <returns>The task that represents the asynchronous receive operation. 
        /// The value of the task's result is null if the connection is dropped; otherwise,
        /// the received <see cref="IClientMessage"/>.</returns>
        Task<IClientMessage> ReceiveClientMessageAsync();

        /// <summary>
        /// Receives the <see cref="ActionMessage"/> asynchronously.
        /// The task's result is null if the connection is dropped.
        /// </summary>
        /// <param name="step">The simulation step which a received <see cref="ActionMessage"/> must have.</param>
        /// <returns>The task that represents the asynchronous receive operation. 
        /// The value of the task's result is null if the connection is dropped or invalid message
        /// is received; otherwise, the received <see cref="ActionMessage"/>.</returns>
        Task<ActionMessage> ReceiveActionMessageAsync(int step);
    }
}