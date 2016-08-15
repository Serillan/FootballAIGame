using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.ApiForWeb;
using FootballAIGameServer.Models;

namespace FootballAIGameServer
{
    class Entry
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(GameServerService));
            host.Open(); // start service
            ConnectionManager manager = new ConnectionManager();
            manager.StartListening(); // start listening
        }
    }
}
