using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAIGameServer.Messages
{
    public class LoginMessage : ClientMessage
    {
        public string PlayerName { get; set; }

        public string AiName { get; set; }
    }
}
