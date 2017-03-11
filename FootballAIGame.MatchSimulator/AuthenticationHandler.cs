using System.Threading.Tasks;
using FootballAIGame.MatchSimulator.Messages;

namespace FootballAIGame.MatchSimulator
{
    public delegate Task<bool> AuthenticationHandler(LoginMessage message, ClientConnection connection);
}