using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.Messages;
using FootballAIGameServer.Models;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer
{
    public class MatchSimulator
    {
        private ClientConnection Player1Connection { get; set; }
        private ClientConnection Player2Connection { get; set; }
        private GameState GameState { get; set; }
        private Match Match { get; set; }

        public const int NumberOfSimulationSteps = 150;
        public const int PlayerStepTime = 100; // ms

        public MatchSimulator(ClientConnection player1Connection, ClientConnection player2Connection)
        {
            this.Player1Connection = player1Connection;
            this.Player2Connection = player2Connection;
        }

        public async Task SimulateMatch()
        {
            Console.WriteLine($"Start simulation between {Player1Connection.PlayerName}:{Player1Connection.AiName} " +
                              $"and {Player2Connection.PlayerName}:{Player2Connection.AiName}");

            Match match = new Match()
            {
                Time = DateTime.Now,
                Player1Ai = Player1Connection.AiName,
                Player2Ai = Player2Connection.AiName
            };

            // todo
            // check player states and update them
            // 1. check pings
            var ping1 = 100; // todo get
            var ping2 = 100;
            // ask for parameters
            

            var message1 = Player1Connection.ReceiveClientMessageAsync();
            var message2 = Player2Connection.ReceiveClientMessageAsync();

            for (int step = 0; step < NumberOfSimulationSteps; step++) // simulation
            {
                Console.WriteLine(step);

                var getMessage1Task = Task.WhenAny(message1, Task.Delay(ping1 + PlayerStepTime));
                var getMessage2Task = Task.WhenAny(message2, Task.Delay(ping2 + PlayerStepTime));

                var getMessage1Result = await getMessage1Task;
                var getMessage2Result = await getMessage2Task;

                ActionMessage actionMessage1 = null;
                ActionMessage actionMessage2 = null;


                if (getMessage1Result == message1)
                {
                    actionMessage1 = message1.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                        message1 = Player1Connection.ReceiveClientMessageAsync();
                }

                if (getMessage2Result == message2)
                {
                    actionMessage2 = message2.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                        message2 = Player2Connection.ReceiveClientMessageAsync();
                }
                Player1Connection.CancelCurrentReceiving();
                Player2Connection.CancelCurrentReceiving();

                UpdateMatch(actionMessage1, actionMessage2);

            }

            Console.WriteLine($"Ending simulation between {Player1Connection.PlayerName}:{Player1Connection.AiName} " +
                             $"and {Player2Connection.PlayerName}:{Player2Connection.AiName}");
            using (var context = new ApplicationDbContext())
            {
                var player1 = context.Players.SingleOrDefault(p => p.Name == Player1Connection.PlayerName);
                var player2 = context.Players.SingleOrDefault(p => p.Name == Player2Connection.PlayerName);
                player1.PlayerState = PlayerState.Idle;
                player2.PlayerState = PlayerState.Idle;
                match.Player1 = player1;
                match.Player2 = player2;
                match.Score = "0:0";
                match.Goals = "";

                context.Matches.Add(match);

                try
                {
                    context.SaveChanges();
                }
                catch (EntityException ex)
                {
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                }
            }
        }

        public void UpdateMatch(ActionMessage player1Action, ActionMessage player2Action)
        {
            // main function - update game state
        }



    }
}
