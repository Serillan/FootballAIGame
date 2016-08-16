using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.Messages;
using FootballAIGameServer.Models;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer
{
    class MatchSimulator
    {
        private ClientConnection Player1Connection { get; set; }
        private ClientConnection Player2Connection { get; set; }
        private GameState GameState { get; set; }

        public const int NumberOfSimulationSteps = 1500;
        public const int PlayerStepTime = 100; // ms

        public MatchSimulator(ClientConnection player1Connection, ClientConnection player2Connection, 
            GameState gameState)
        {
            this.GameState = gameState;
            this.Player1Connection = player1Connection;
            this.Player2Connection = player2Connection;
        }

        public async Task SimulateMatch()
        {
            // todo
            // check player states and update them
            // 1. check pings
            var ping1 = 100; // todo get
            var ping2 = 100;
            // ask for parameters

            for (int step = 0; step < NumberOfSimulationSteps; step++) // simulation
            {
                var message1 = Player1Connection.ReceiveClientMessageAsync();
                var message2 = Player2Connection.ReceiveClientMessageAsync();

                var getMessage1Task = Task.WhenAny(message1, Task.Delay(ping1 + PlayerStepTime)).ContinueWith(m =>
                {
                    
                });

                var getMessage2Task = Task.WhenAny(message2, Task.Delay(ping2 + PlayerStepTime)).ContinueWith(m =>
                {
                    
                });

                await Task.WhenAll(getMessage1Task, getMessage2Task);
                UpdateMatch((ActionMessage)message1.Result, (ActionMessage)message2.Result);
            }
        }

        public void UpdateMatch(ActionMessage player1Action, ActionMessage player2Action)
        {
            // main function - update game state
        }



    }
}
