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
        string WantsToPlay(string userName, string ai);

        [OperationContract]
        string StartGame(string userName1, string ai1, string userName2, string ai2);
    }
}
