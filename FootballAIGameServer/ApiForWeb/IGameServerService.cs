using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FootballAIGameServer.ApiForWeb
{
    [ServiceContract]
    public interface IGameServerService
    {
        [OperationContract]
        string WantsToPlay(string userName, string ai);

        [OperationContract]
        string StartGame(string userName1, string ai1, string userName2, string ai2);

        [OperationContract]
        void CancelMatch(string playerName);

        [OperationContract]
        void CancelLooking(string playername);
    }
}
