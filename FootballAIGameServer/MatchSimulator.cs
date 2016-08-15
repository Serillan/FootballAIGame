using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.Models;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer
{
    class MatchSimulator
    {
        private ClientConnection Player1Connection { get; set; }
        private ClientConnection Player2Connection { get; set; }
        private GameState GameState { get; set; }

        public MatchSimulator(ClientConnection player1Connection, ClientConnection player2Connection, 
            GameState gameState)
        {
            this.GameState = gameState;
            this.Player1Connection = player1Connection;
            this.Player2Connection = player2Connection;
        }

        public void Start()
        {
            
        }



    }
}
