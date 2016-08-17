using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FootballAIGameServer.ApiForWeb
{
    public class GameServerService : IGameServerService
    {
        public void WantsToPlay(string userId, string ai)
        {
            Console.WriteLine("wants to player");
        }

        public void StartGame(string userId1, string userId2)
        {
            Console.WriteLine("start game");
        }
    }
}
