using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
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
        public bool Player1CancelRequested { get; set; }
        public bool Player2CancelRequested { get; set; }

        public const int NumberOfSimulationSteps = 1500;
        public const int PlayerTimeForOneStep = 100; // ms

        private GameState GameState { get; set; }
        private Match Match { get; set; }
        private int Ping1 { get; set; }
        private int Ping2 { get; set; }
        private List<float> Data { get; set; }
        private Task<ClientMessage> Message1 { get; set; }
        private Task<ClientMessage> Message2 { get; set; }
        private Random Random { get; set; }

        public static List<MatchSimulator> RunningSimulations { get; set; }

        public MatchSimulator(ClientConnection player1Connection, ClientConnection player2Connection)
        {
            this.Player1Connection = player1Connection;
            this.Player2Connection = player2Connection;
            Random = new Random();
        }

        public async Task ProcessGettingParameters()
        {
            var getParameters1 = GetPlayersParameters(Player1Connection);
            var getParameters2 = GetPlayersParameters(Player2Connection);
            var result1 = await Task.WhenAny(getParameters1, Task.Delay(500 + Ping1));
            var result2 = await Task.WhenAny(getParameters2, Task.Delay(500 + Ping2));

            // player 1
            if (result1 == getParameters1 && !getParameters1.IsFaulted)
            {
                for (var i = 0; i < 11; i++)
                {
                    var par = getParameters1.Result[i];
                    if (par.Speed <= 0.4001 && par.KickPower <= 0.4001 &&
                        par.Possesion <= 0.4001 && par.Precision <= 0.4001 &&
                        par.Speed + par.KickPower + par.Possesion + par.Precision - 1 <= 0.01)
                    {
                        GameState.FootballPlayers[i] = getParameters1.Result[i];
                    }
                    else // default parameters
                    {
                        GameState.FootballPlayers[i] = new FootballPlayer()
                        {
                            Speed = 0.25f,
                            KickPower = 0.25f,
                            Possesion = 0.25f,
                            Precision = 0.25f
                        };
                    }
                }
            }
            else // default parameters for all players
            {
                for (var i = 0; i < 11; i++)
                    GameState.FootballPlayers[i] = new FootballPlayer()
                    {
                        Speed = 0.25f,
                        KickPower = 0.25f,
                        Possesion = 0.25f,
                        Precision = 0.25f
                    };
            }

            // player 2
            if (result2 == getParameters2 && !getParameters2.IsFaulted)
            {
                for (var i = 0; i < 11; i++)
                {
                    var par = getParameters2.Result[i];
                    if (par.Speed <= 0.4001 && par.KickPower <= 0.4001 &&
                        par.Possesion <= 0.4001 && par.Precision <= 0.4001 &&
                        par.Speed + par.KickPower + par.Possesion + par.Precision - 1 <= 0.01)
                    {
                        GameState.FootballPlayers[i + 11] = getParameters2.Result[i];
                    }
                    else // default parameters
                    {
                        GameState.FootballPlayers[i + 11] = new FootballPlayer()
                        {
                            Speed = 0.25f,
                            KickPower = 0.25f,
                            Possesion = 0.25f,
                            Precision = 0.25f
                        };
                    }
                }
            }
            else // default parameters for all players
            {
                for (var i = 0; i < 11; i++)
                    GameState.FootballPlayers[i + 11] = new FootballPlayer()
                    {
                        Speed = 0.25f,
                        KickPower = 0.25f,
                        Possesion = 0.25f,
                        Precision = 0.25f
                    };
            }
        }

        public async Task ProcessSimulationStart()
        {
            if (RunningSimulations == null)
                RunningSimulations = new List<MatchSimulator>();
            RunningSimulations.Add(this);

            Console.WriteLine($"Starting simulation between {Player1Connection.PlayerName}:{Player1Connection.AiName} " +
                              $"and {Player2Connection.PlayerName}:{Player2Connection.AiName}");

            Match = new Match()
            {
                Time = DateTime.Now,
                Player1Ai = Player1Connection.AiName,
                Player2Ai = Player2Connection.AiName
            };

            // 1. check pings -todo timeout check
            Ping1 = Player1Connection.PingTimeAverage();
            Ping2 = Player2Connection.PingTimeAverage();
            Console.WriteLine($"{Player1Connection.PlayerName} ping = {Ping1}\n" +
                              $"{Player2Connection.PlayerName} ping = {Ping2}");

            Data = new List<float>();

            // todo
            // check player states and update them
            GameState = new GameState
            {
                Ball = new Ball(),
                FootballPlayers = new FootballPlayer[22]
            };

            await ProcessGettingParameters();

            Message1 = Player1Connection.ReceiveClientMessageAsync();
            Message2 = Player2Connection.ReceiveClientMessageAsync();
        }

        public void SetStartingPositions(int whoHasBall)
        {
            var players = GameState.FootballPlayers;

            /* TEAM 1 */
            // goalie
            players[0].X = 11;
            players[0].Y = 75f / 2;

            // defenders positions
            players[1].X = 16.5f;
            players[1].Y = 15;
            players[2].X = 16.5f;
            players[2].Y = 75 / 2f - 12;
            players[3].X = 16.5f;
            players[3].Y = 75 / 2f + 12;
            players[4].X = 16.5f;
            players[4].Y = 50;

            // midfielders
            players[5].X = 33;
            players[5].Y = 75 / 2f - 12;
            players[6].X = 33;
            players[6].Y = 75 / 2f;
            players[7].X = 33;
            players[7].Y = 75 / 2f + 12;
            players[8].X = 33;
            players[8].Y = 15;

            // forwards
            players[9].X = 45;
            players[9].Y = 75 / 2f - 6;
            players[10].X = 45;
            players[10].Y = 75 / 2f + 6;

            /* TEAM 2 (symmetrical) */
            for (var i = 0; i < 11; i++)
            {
                players[i + 11].X = 110 - players[i].X;
                players[i + 11].Y = players[i].Y;
            }

            // noone is moving
            foreach (var player in players)
                player.VectorX = player.VectorY = 0;

            // ball
            GameState.Ball.X = 55;
            GameState.Ball.Y = 75 / 2f;
            GameState.Ball.VectorX = 0f;
            GameState.Ball.VectorY = 0f;

            /* ball winner */
            if (whoHasBall == 1)
            {
                players[10].X = 54;
                players[10].Y = 75 / 2f;
            }
            else
            {
                players[21].X = 56;
                players[21].Y = 75 / 2f;
            }
        }

        public async Task ProcessSimulationStep(int step)
        {
            Console.WriteLine(step);

            ActionMessage actionMessage1 = null;
            ActionMessage actionMessage2 = null;

            if (Player1CancelRequested || Player2CancelRequested || !Player1Connection.IsActive || !Player2Connection.IsActive)
                return;
            try
            {
                await Player1Connection.SendAsync("GET ACTION");
                await Player1Connection.SendAsync(GameState);

                await Player2Connection.SendAsync("GET ACTION");
                await Player2Connection.SendAsync(GameState);


                var getMessage1Task = Task.WhenAny(Message1, Task.Delay(Ping1 + PlayerTimeForOneStep));
                var getMessage2Task = Task.WhenAny(Message2, Task.Delay(Ping2 + PlayerTimeForOneStep));

                var getMessage1Result = await getMessage1Task;
                var getMessage2Result = await getMessage2Task;




                if (Message1.IsFaulted || !Player1Connection.IsActive || Message2.IsFaulted ||
                    !Player2Connection.IsActive)
                {
                    return;
                }

                if (getMessage1Result == Message1)
                {
                    actionMessage1 = Message1.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                    {
                        Message1 = Player1Connection.ReceiveClientMessageAsync();
                    }
                }

                if (getMessage2Result == Message2 && !Message2.IsFaulted)
                {
                    actionMessage2 = Message2.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                    {
                        Message2 = Player2Connection.ReceiveClientMessageAsync();
                    }
                }
            }
            catch (IOException) // if player1 or player2 has disconnected
            {
                return;
            }


            UpdateMatch(actionMessage1, actionMessage2);
            SaveState();
        }

        public async Task SimulateMatch()
        {
            await ProcessSimulationStart();

            var firstBall = Random.Next(1, 2);

            // first half
            try
            {
                SetStartingPositions(firstBall);
                for (var step = 0; step < NumberOfSimulationSteps / 2; step++)
                {
                    await ProcessSimulationStep(step);
                }

                // second half
                SetStartingPositions(firstBall == 1 ? 2 : 1);
                for (var step = NumberOfSimulationSteps / 2; step < NumberOfSimulationSteps; step++)
                {
                    await ProcessSimulationStep(step);
                }

                ProcessEnding();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcessEnding()
        {
            Console.WriteLine($"Ending simulation between {Player1Connection.PlayerName}:{Player1Connection.AiName} " +
                             $"and {Player2Connection.PlayerName}:{Player2Connection.AiName}");
            using (var context = new ApplicationDbContext())
            {
                var player1 = context.Players.SingleOrDefault(p => p.Name == Player1Connection.PlayerName);
                var player2 = context.Players.SingleOrDefault(p => p.Name == Player2Connection.PlayerName);
                player1.PlayerState = PlayerState.Idle;
                player2.PlayerState = PlayerState.Idle;
                Match.Player1 = player1;
                Match.Player2 = player2;
                Match.Score = "0:0";
                Match.Goals = "";

                if (Message1.IsFaulted || !Player1Connection.IsActive)
                {
                    Match.Winner = 2;
                    Match.Player1ErrorLog += "Player AI has disconnected.;";
                }
                if (Message2.IsFaulted || !Player2Connection.IsActive)
                {
                    Match.Winner = 1;
                    Match.Player2ErrorLog += "Player AI has disconnected.;";
                }

                if (Player1CancelRequested)
                {
                    Match.Winner = 2;
                    Match.Player1ErrorLog += "Player has cancelled the match.;";
                }
                if (Player2CancelRequested)
                {
                    Match.Winner = 1;
                    Match.Player2ErrorLog += "Player has cancelled the match.;";
                }

                var matchByteData = new byte[Data.Count * 4];
                Buffer.BlockCopy(Data.ToArray(), 0, matchByteData, 0, Data.Count * 4);
                Match.MatchData = matchByteData;

                context.Matches.Add(Match);

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
                currentStateData[2 + 2 * i] = GameState.FootballPlayers[i].X;
                currentStateData[2 + 2 * i + 1] = GameState.FootballPlayers[i].Y;
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
            if (player1Action != null)
            {
                for (var i = 0; i < 11; i++)
                {
                    GameState.FootballPlayers[i].X += player1Action.PlayerActions[i].VectorX;
                    GameState.FootballPlayers[i].Y += player1Action.PlayerActions[i].VectorY;
                }
            }
            else
            {
                // default (nothing for now)
                Console.WriteLine(Player1Connection.PlayerName + " timeout.");
            }
            if (player2Action != null)
            {
                for (var i = 0; i < 11; i++)
                {
                    GameState.FootballPlayers[i + 11].X += player2Action.PlayerActions[i].VectorX;
                    GameState.FootballPlayers[i + 11].Y += player2Action.PlayerActions[i].VectorY;
                }
            }
            else
            {
                // default (nothing for now)
                Console.WriteLine(Player2Connection.PlayerName + " timeout.");
            }
        }



    }
}
