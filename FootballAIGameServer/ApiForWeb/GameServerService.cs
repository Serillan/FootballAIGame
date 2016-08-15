using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FootballAIGameServer.ApiForWeb
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GameService" in both code and config file together.
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
