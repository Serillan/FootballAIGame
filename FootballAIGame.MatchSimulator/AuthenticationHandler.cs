using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.Messages;

namespace FootballAIGame.MatchSimulation
{
    public delegate Task<bool> AuthenticationHandler(LoginMessage message, ClientConnection connection);
}