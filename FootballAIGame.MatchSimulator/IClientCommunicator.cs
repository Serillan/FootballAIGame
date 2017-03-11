using System.Threading.Tasks;
using FootballAIGame.MatchSimulator.Messages;
using FootballAIGame.MatchSimulator.SimulationEntities;

namespace FootballAIGame.MatchSimulator
{
    public interface IClientCommunicator
    {
        string PlayerName { get; set; }

        bool IsActive { get; set; }

        bool IsConnected { get; }

        Task SendAsync(string message);

        Task SendAsync(GameState gameState, int playerNumber);

        Task<bool> TrySendAsync(string message);

        Task<bool> TrySendAsync(GameState gameState, int playerNumber);

        Task<ClientMessage> ReceiveClientMessageAsync();

        Task<ActionMessage> ReceiveActionMessageAsync(int step);
    }
}
