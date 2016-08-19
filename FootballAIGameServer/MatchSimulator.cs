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
        public ClientConnection Player1Connection { get; set; }
        public ClientConnection Player2Connection { get; set; }
        private GameState GameState { get; set; }
        private Match Match { get; set; }
        private int Ping1 { get; set; }
        private int Ping2 { get; set; }
        private List<float> Data { get; set; }

        public bool Player1CancelRequested { get; set; }
        public bool Player2CancelRequested { get; set; }

        public const int NumberOfSimulationSteps = 150;
        public const int PlayerStepTime = 100; // ms

        public static List<MatchSimulator> RunningSimulations { get; set; }

        public MatchSimulator(ClientConnection player1Connection, ClientConnection player2Connection)
        {
            this.Player1Connection = player1Connection;
            this.Player2Connection = player2Connection;
        }

        public async Task ProcessGettingParameters()
        {
            var getParameters1 = GetPlayersParameters(Player1Connection); // todo delay
            var getParameters2 = GetPlayersParameters(Player2Connection);
            var result1 = await Task.WhenAny(getParameters1, Task.Delay(500 + Ping1));
            var result2 = await Task.WhenAny(getParameters2, Task.Delay(500 + Ping2));

            if (result1 == getParameters1 && !getParameters1.IsFaulted)
            {
                for (var i = 0; i < 11; i++)
                    GameState.FootballPlayers[i] = getParameters1.Result[i]; // todo checking
            }
            else
            {
                for (var i = 0; i < 11; i++)
                    GameState.FootballPlayers[i] = new FootballPlayer()
                    {
                        Speed = 0.2f,
                        KickPower = 0.2f,
                        Possesion = 0.2f,
                        Precision = 0.2f
                    };
            }

            if (result2 == getParameters2 && !getParameters2.IsFaulted)
            {
                for (var i = 0; i < 11; i++)
                    GameState.FootballPlayers[i + 11] = getParameters2.Result[i]; // todo checking
            }
            else
            {
                for (var i = 0; i < 11; i++)
                    GameState.FootballPlayers[i + 11] = new FootballPlayer()
                    {
                        Speed = 0.2f,
                        KickPower = 0.2f,
                        Possesion = 0.2f,
                        Precision = 0.2f
                    };
            }
        }

        public async Task SimulateMatch()
        {
            if (RunningSimulations == null)
                RunningSimulations = new List<MatchSimulator>();
            RunningSimulations.Add(this);

            Console.WriteLine($"Start simulation between {Player1Connection.PlayerName}:{Player1Connection.AiName} " +
                              $"and {Player2Connection.PlayerName}:{Player2Connection.AiName}");

            Match match = new Match()
            {
                Time = DateTime.Now,
                Player1Ai = Player1Connection.AiName,
                Player2Ai = Player2Connection.AiName
            };
            Data = new List<float>();

            // 1. check pings
            Ping1 = 100; // todo get
            Ping2 = 100;

            // todo
            // check player states and update them
            GameState = new GameState
            {
                Ball = new Ball()
                {
                    X = 50,
                    Y = 50,
                    VectorX = 0,
                    VectorY = 0
                },
                FootballPlayers = new FootballPlayer[22]
            };

            await ProcessGettingParameters();

            var message1 = Player1Connection.ReceiveClientMessageAsync();
            var message2 = Player2Connection.ReceiveClientMessageAsync();

            for (var step = 0; step < NumberOfSimulationSteps; step++) // simulation
            {
                Console.WriteLine(step);

                if (Player1CancelRequested || Player2CancelRequested)
                    break;

                await Player1Connection.SendAsync("GET ACTION");
                await Player1Connection.SendAsync(GameState);

                await Player2Connection.SendAsync("GET ACTION");
                await Player2Connection.SendAsync(GameState);


                var getMessage1Task = Task.WhenAny(message1, Task.Delay(Ping1 + PlayerStepTime));
                var getMessage2Task = Task.WhenAny(message2, Task.Delay(Ping2 + PlayerStepTime));

                var getMessage1Result = await getMessage1Task;
                var getMessage2Result = await getMessage2Task;

                ActionMessage actionMessage1 = null;
                ActionMessage actionMessage2 = null;

                if (message1.IsFaulted || !Player1Connection.IsActive || message2.IsFaulted || !Player2Connection.IsActive)
                {
                    break;
                }

                if (getMessage1Result == message1)
                {
                    actionMessage1 = message1.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                        message1 = Player1Connection.ReceiveClientMessageAsync();
                }

                if (getMessage2Result == message2 && !message2.IsFaulted)
                {
                    actionMessage2 = message2.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                        message2 = Player2Connection.ReceiveClientMessageAsync();
                }

                UpdateMatch(actionMessage1, actionMessage2);
                SaveState();
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

                if (message1.IsFaulted || !Player1Connection.IsActive)
                {
                    match.Winner = 2;
                    match.Player1ErrorLog += "Player AI has disconnected.;";
                }
                if (message2.IsFaulted || !Player2Connection.IsActive)
                {
                    match.Winner = 1;
                    match.Player2ErrorLog += "Player AI has disconnected.;";
                }

                if (Player1CancelRequested)
                {
                    match.Winner = 2;
                    match.Player1ErrorLog += "Player has cancelled the match.;";
                }
                if (Player2CancelRequested)
                {
                    match.Winner = 1;
                    match.Player2ErrorLog += "Player has cancelled the match.;";
                }

                byte[] matchByteData = new byte[Data.Count * 4];
                Buffer.BlockCopy(Data.ToArray(), 0, matchByteData, 0, Data.Count);
                match.MatchData = matchByteData;

                context.Matches.Add(match);

                context.SaveChanges();

                RunningSimulations.Remove(this);
            }
        }

        private void SaveState()
        {
            float[] currentStateData = new float[46];
            currentStateData[0] = GameState.Ball.X;
            currentStateData[1] = GameState.Ball.Y;

            for (var i = 0; i < 22; i++)
            {
                currentStateData[2 + 2*i] = GameState.FootballPlayers[i].X;
                currentStateData[2 + 2*i + 1] = GameState.FootballPlayers[i].Y;
            }

            foreach (var num in currentStateData)
                Data.Add(num);
        }

        private async Task<FootballPlayer[]> GetPlayersParameters(ClientConnection playerConnection)
        {
            await playerConnection.SendAsync("GET PARAMETERS");
            var parametersMessage = await playerConnection.ReceiveClientMessageAsync();
            return ((ParametersMessage)parametersMessage).Players;
        }

        public void UpdateMatch(ActionMessage player1Action, ActionMessage player2Action)
        {
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i].X += player1Action.PlayerActions[i].VectorX;
                GameState.FootballPlayers[i].Y += player1Action.PlayerActions[i].VectorY;
            }
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i + 11].X += player2Action.PlayerActions[i].VectorX;
                GameState.FootballPlayers[i + 11].Y += player2Action.PlayerActions[i].VectorY;
            }
        }



    }
}
