using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FootballAIGameServer.ApiForWeb
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGameService" in both code and config file together.
    [ServiceContract]
    public interface IGameServerService
    {
        [OperationContract]
        void WantsToPlay(string userId, string ai);

        [OperationContract]
        void StartGame(string userId1, string userId2);
    }
}
