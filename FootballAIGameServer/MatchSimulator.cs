using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using FootballAIGameServer.CustomDataTypes;
using FootballAIGameServer.Messages;
using FootballAIGameServer.Models;
using FootballAIGameServer.SimulationEntities;

namespace FootballAIGameServer
{
    public class MatchSimulator
    {

        public const int NumberOfSimulationSteps = 1500;
        public const int PlayerTimeForOneStep = 100; // [ms]
        public const int StepInterval = 200; // [ms]
        public const double MaxAcceleration = 3; // [m/s/s]
        public const double MaxBallSpeed = 15; // [m/s]
        public const double BallDecerelation = 1.5; // [m/s/s]
        public const double MinimalOpponentLengthFromCornerKick = 6; // [m]
        public const int MaximumNumberOfSameKindErrorInLog = 5;

        public ClientConnection Player1Connection { get; set; }
        public ClientConnection Player2Connection { get; set; }
        public bool Player1CancelRequested { get; set; }
        public bool Player2CancelRequested { get; set; }

        public static List<MatchSimulator> RunningSimulations { get; set; }
        public static Random Random { get; set; }

        private GameState GameState { get; set; }
        private Match Match { get; set; }
        private int Ping1 { get; set; }
        private int Ping2 { get; set; }
        private List<float> Data { get; set; }
        private Task<ClientMessage> Message1 { get; set; }
        private Task<ClientMessage> Message2 { get; set; }
        private FootballPlayer LastKicker { get; set; }
        private int WhoIsOnLeft { get; set; }
        private int CurrentStep { get; set; }
        private int CurrentScore1 { get; set; }
        private int CurrentScore2 { get; set; }
        private string CurrentTime
        {
            get
            {
                var totalSeconds = CurrentStep * StepInterval / 1000;
                var minutes = totalSeconds / 60;
                var seconds = totalSeconds - minutes * 60;
                return $"{minutes}:{seconds}";
            }
        }
        private int NumberOfAccelerationCorrections1 { get; set; }
        private int NumberOfAccelerationCorrections2 { get; set; }
        private int NumberOfSpeedCorrections1 { get; set; }
        private int NumberOfSpeedCorrections2 { get; set; }

        public MatchSimulator(ClientConnection player1Connection, ClientConnection player2Connection)
        {
            this.Player1Connection = player1Connection;
            this.Player2Connection = player2Connection;
        }

        public async Task SimulateMatch()
        {
            await ProcessSimulationStart();

            var firstBall = Random.Next(1, 3); // max is excluded

            // first half
            try
            {
                WhoIsOnLeft = 1;
                SetStartingPositions(firstBall);
                SaveState(); // save starting state

                for (var step = 0; step < NumberOfSimulationSteps / 2; step++)
                {
                    CurrentStep = step;
                    await ProcessSimulationStep(step);
                    if (step % 100 == 0)
                        Console.WriteLine(step);
                }

                // second half
                WhoIsOnLeft = 2;
                SetStartingPositions(firstBall == 1 ? 2 : 1);
                for (var step = NumberOfSimulationSteps / 2; step < NumberOfSimulationSteps; step++)
                {
                    CurrentStep = step;
                    await ProcessSimulationStep(step);
                    if (step % 100 == 0)
                        Console.WriteLine(step);
                }

                ProcessSimulationEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("simulation exception");
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ProcessSimulationStart()
        {
            if (RunningSimulations == null)
                RunningSimulations = new List<MatchSimulator>();
            RunningSimulations.Add(this);

            Player1Connection.IsInMatch = true;
            Player2Connection.IsInMatch = true;

            Console.WriteLine($"Starting simulation between {Player1Connection.PlayerName}:{Player1Connection.AiName} " +
                              $"and {Player2Connection.PlayerName}:{Player2Connection.AiName}");

            Match = new Match()
            {
                Time = DateTime.Now,
                Player1Ai = Player1Connection.AiName,
                Player2Ai = Player2Connection.AiName,
                Player1ErrorLog = "",
                Player2ErrorLog = "",
                Goals = ""
            };

            // 1. check pings
            Ping1 = Player1Connection.PingTimeAverage();
            Ping2 = Player2Connection.PingTimeAverage();
            //Console.WriteLine($"{Player1Connection.PlayerName} ping = {Ping1}\n" +
            //                  $"{Player2Connection.PlayerName} ping = {Ping2}");

            Data = new List<float>();

            GameState = new GameState
            {
                Ball = new Ball(),
                FootballPlayers = new FootballPlayer[22]
            };

            await ProcessGettingParameters();

            Message1 = Player1Connection.ReceiveClientMessageAsync();
            Message2 = Player2Connection.ReceiveClientMessageAsync();
        }

        private async Task ProcessSimulationStep(int step)
        {
            ActionMessage actionMessage1 = null;
            ActionMessage actionMessage2 = null;

            if (Player1CancelRequested || Player2CancelRequested || !Player1Connection.IsActive || !Player2Connection.IsActive)
                return;
            try
            {
                await Player1Connection.SendAsync("GET ACTION");
                await Player1Connection.SendAsync(GameState, 1);

                await Player2Connection.SendAsync("GET ACTION");
                await Player2Connection.SendAsync(GameState, 2);


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
                Console.WriteLine("step exception");
                return;
            }


            UpdateMatch(actionMessage1, actionMessage2);
            SaveState();
        }

        private void ProcessSimulationEnd()
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
                Match.Score = $"{CurrentScore1}:{CurrentScore2}";
                Match.Winner = CurrentScore1 > CurrentScore2 ? 1 : CurrentScore1 < CurrentScore2 ? 2 : 0;

                if (Message1.IsFaulted || !Player1Connection.IsActive)
                {
                    Match.Winner = 2;
                    Match.Player1ErrorLog += $"{CurrentTime} - Player AI has disconnected.;";
                }
                if (Message2.IsFaulted || !Player2Connection.IsActive)
                {
                    Match.Winner = 1;
                    Match.Player2ErrorLog += $"{CurrentTime} - Player AI has disconnected.;";
                }

                if (Player1CancelRequested)
                {
                    Match.Winner = 2;
                    Match.Player1ErrorLog += $"{CurrentTime} - Player has cancelled the match.;";
                }
                if (Player2CancelRequested)
                {
                    Match.Winner = 1;
                    Match.Player2ErrorLog += $"{CurrentTime} - Player has cancelled the match.;";
                }

                var matchByteData = new byte[Data.Count * 4];
                Buffer.BlockCopy(Data.ToArray(), 0, matchByteData, 0, Data.Count * 4);
                Match.MatchData = matchByteData;

                context.Matches.Add(Match);

                context.SaveChanges();

                RunningSimulations.Remove(this);
                Player1Connection.IsInMatch = false;
                Player2Connection.IsInMatch = false;
            }
        }

        private async Task ProcessGettingParameters()
        {
            var getParameters1 = GetPlayerParameters(Player1Connection);
            var getParameters2 = GetPlayerParameters(Player2Connection);
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

        private void SetStartingPositions(int whoHasBall)
        {
            var players = GameState.FootballPlayers;

            /* TEAM 1 */
            // goalie
            players[0].Position.X = 11;
            players[0].Position.Y = 75f / 2;

            // defenders positions
            players[1].Position.X = 16.5f;
            players[1].Position.Y = 15;
            players[2].Position.X = 16.5f;
            players[2].Position.Y = 75 / 2f - 12;
            players[3].Position.X = 16.5f;
            players[3].Position.Y = 75 / 2f + 12;
            players[4].Position.X = 16.5f;
            players[4].Position.Y = 60;

            // midfielders
            players[5].Position.X = 33;
            players[5].Position.Y = 75 / 2f - 12;
            players[6].Position.X = 33;
            players[6].Position.Y = 75 / 2f;
            players[7].Position.X = 33;
            players[7].Position.Y = 75 / 2f + 12;
            players[8].Position.X = 33;
            players[8].Position.Y = 15;

            // forwards
            players[9].Position.X = 45;
            players[9].Position.Y = 75 / 2f - 6;
            players[10].Position.X = 45;
            players[10].Position.Y = 75 / 2f + 6;

            /* TEAM 2 (symmetrical) */
            for (var i = 0; i < 11; i++)
            {
                players[i + 11].Position.X = 110 - players[i].Position.X;
                players[i + 11].Position.Y = players[i].Position.Y;
            }

            // noone is moving
            foreach (var player in players)
                player.Movement.X = player.Movement.Y = 0;

            // ball
            GameState.Ball.Position.X = 55;
            GameState.Ball.Position.Y = 75 / 2f;
            GameState.Ball.Movement.X = 0f;
            GameState.Ball.Movement.Y = 0f;

            /* ball winner */
            if ((whoHasBall == 1 && WhoIsOnLeft == 1) || (whoHasBall == 2 && WhoIsOnLeft == 2))
            {
                players[10].Position.X = 54;
                players[10].Position.Y = 75 / 2f;
            }
            else
            {
                players[21].Position.X = 56;
                players[21].Position.Y = 75 / 2f;
            }

            if (WhoIsOnLeft == 2)
            {
                // switch
                for (var i = 0; i < 11; i++)
                {
                    var tempX = players[i].Position.X;
                    var tempY = players[i].Position.Y;
                    players[i].Position.X = players[i + 11].Position.X;
                    players[i].Position.Y = players[i + 11].Position.Y;
                    players[i + 11].Position.X = tempX;
                    players[i + 11].Position.Y = tempY;
                }
            }
        }

        private void UpdateMatch(ActionMessage player1Action, ActionMessage player2Action)
        {
            UpdatePlayers(player1Action, player2Action);
            UpdateBall(player1Action, player2Action);
            HandleOuts();
            HandleGoals();
        }

        private void SaveState()
        {
            var currentStateData = new float[46];
            currentStateData[0] = (float)GameState.Ball.Position.X;
            currentStateData[1] = (float)GameState.Ball.Position.Y;

            for (var i = 0; i < 22; i++)
            {
                currentStateData[2 + 2 * i] = (float)GameState.FootballPlayers[i].Position.X;
                currentStateData[2 + 2 * i + 1] = (float)GameState.FootballPlayers[i].Position.Y;
            }

            foreach (var num in currentStateData)
                Data.Add(num);
        }

        private static async Task<FootballPlayer[]> GetPlayerParameters(ClientConnection playerConnection)
        {
            ClientMessage message;

            while (true)
            {
                await playerConnection.SendAsync("GET PARAMETERS");
                message = await playerConnection.ReceiveClientMessageAsync();
                if (message is ParametersMessage)
                    break;
            }

            return ((ParametersMessage)message).Players;
        }

        private void UpdatePlayers(ActionMessage player1Action, ActionMessage player2Action)
        {
            if (player1Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i];
                    var action = player1Action.PlayerActions[i];

                    var acceleration = new Vector(action.Movement.X - player.Movement.X,
                        action.Movement.Y - player.Movement.Y);

                    var accelerationValue = acceleration.Length * 1000 / StepInterval; // [m/s]

                    if (accelerationValue > MaxAcceleration)
                    {
                        var q = MaxAcceleration / accelerationValue;
                        var fixedAcceleration = new Vector(acceleration.X * q, acceleration.Y * q);

                        action.Movement.X = (float)(player.Movement.X + fixedAcceleration.X);
                        action.Movement.Y = (float)(player.Movement.Y + fixedAcceleration.Y);

                        if (NumberOfAccelerationCorrections1++ < MaximumNumberOfSameKindErrorInLog)
                            Match.Player1ErrorLog += $"{CurrentTime} - Player{i} acceleration correction.;";
                        
                    }


                    player.Movement.X = action.Movement.X;
                    player.Movement.Y = action.Movement.Y;
                    var newSpeed = player.CurrentSpeed;

                    if (newSpeed > player.MaxSpeed)
                    {
                        // too high speed
                        player.Movement.X *= (float)(player.MaxSpeed / newSpeed);
                        player.Movement.Y *= (float)(player.MaxSpeed / newSpeed);

                        if (NumberOfSpeedCorrections1++ < MaximumNumberOfSameKindErrorInLog)
                            Match.Player1ErrorLog += $"{CurrentTime} - Player{i} speed correction.;";
                    }

                    // apply
                    player.Position.X += player.Movement.X;
                    player.Position.Y += player.Movement.Y;
                }
            }
            else
            {
                // default (nothing for now)
                Console.WriteLine(Player1Connection.PlayerName + " timeout.");
            }

            if (player2Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i + 11];
                    var action = player2Action.PlayerActions[i];

                    var acceleration = new Vector(action.Movement.X - player.Movement.X,
                        action.Movement.Y - player.Movement.Y);

                    var accelerationValue = acceleration.Length * 1000 / StepInterval; // [m/s]

                    if (accelerationValue > MaxAcceleration)
                    {
                        var q = MaxAcceleration / accelerationValue;
                        var fixedAcceleration = new Vector(acceleration.X * q, acceleration.Y * q);

                        action.Movement.X = (float)(player.Movement.X + fixedAcceleration.X);
                        action.Movement.Y = (float)(player.Movement.Y + fixedAcceleration.Y);

                        if (NumberOfAccelerationCorrections2++ < MaximumNumberOfSameKindErrorInLog)
                            Match.Player2ErrorLog += $"{CurrentTime} - Player{i} acceleration correction.;";

                    }


                    player.Movement.X = action.Movement.X;
                    player.Movement.Y = action.Movement.Y;
                    var newSpeed = player.CurrentSpeed;

                    if (newSpeed > player.MaxSpeed)
                    {
                        player.Movement.X *= (float)(player.MaxSpeed / newSpeed);
                        player.Movement.Y *= (float)(player.MaxSpeed / newSpeed);

                        if (NumberOfSpeedCorrections2++ < MaximumNumberOfSameKindErrorInLog)
                            Match.Player2ErrorLog += $"{CurrentTime} - Player{i} speed correction.;";
                    }

                    // apply
                    player.Position.X += player.Movement.X;
                    player.Position.Y += player.Movement.Y;

                }
            }
            else
            {
                // default (nothing for now)
                Console.WriteLine(Player2Connection.PlayerName + " timeout.");
            }
        }

        private void UpdateBall(ActionMessage player1Action, ActionMessage player2Action)
        {
            // add kick actions to players
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i].Kick.X = player1Action?.PlayerActions[i].Kick.X ?? 0;
                GameState.FootballPlayers[i].Kick.Y = player1Action?.PlayerActions[i].Kick.Y ?? 0;
            }
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i + 11].Kick.X = player2Action?.PlayerActions[i].Kick.X ?? 0;
                GameState.FootballPlayers[i + 11].Kick.Y = player2Action?.PlayerActions[i].Kick.Y ?? 0;
            }

            var playersNearBallKicking = GameState.FootballPlayers.Where(
                p => Vector.DistanceBetween(GameState.Ball.Position, p.Position) <= 2 && (p.Kick.X != 0 || p.Kick.Y != 0));

            var kickWinner = GetKickWinner(playersNearBallKicking.ToArray());

            // apply kick
            if (kickWinner != null)
            {

                // check whether the ball was shot or shot on target
                UpdateStoppedShots(kickWinner);

                LastKicker = kickWinner;
                GameState.Ball.Movement.X = kickWinner.Kick.X;
                GameState.Ball.Movement.Y = kickWinner.Kick.Y;

                // deviation of the kick
                var angleDevation = (0.4 - kickWinner.Precision) * (Random.NextDouble() * 2 - 1);

                // rotation applied (deviation)
                GameState.Ball.Movement.X = (float)(Math.Cos(angleDevation) * GameState.Ball.Movement.X -
                                                Math.Sin(angleDevation) * GameState.Ball.Movement.Y);
                GameState.Ball.Movement.Y = (float)(Math.Sin(angleDevation) * GameState.Ball.Movement.X +
                                                Math.Cos(angleDevation) * GameState.Ball.Movement.Y);

                var newSpeed = GameState.Ball.CurrentSpeed;
                var maxAllowedSpeed = 15 + LastKicker.KickPower * 10;
                if (newSpeed > maxAllowedSpeed)
                {
                    GameState.Ball.Movement.X *= (float)(maxAllowedSpeed / newSpeed);
                    GameState.Ball.Movement.Y *= (float)(maxAllowedSpeed / newSpeed);
                }
            }

            // ball deceralation
            var ratio = (GameState.Ball.CurrentSpeed - (BallDecerelation * StepInterval / 1000)) /
                GameState.Ball.CurrentSpeed;
            if (ratio < 0)
                ratio = 0;
            GameState.Ball.Movement.X *= (float)ratio;
            GameState.Ball.Movement.Y *= (float)ratio;

            // update ball position
            GameState.Ball.Position.X += GameState.Ball.Movement.X;
            GameState.Ball.Position.Y += GameState.Ball.Movement.Y;
        }

        private void HandleOuts()
        {
            var lastTeam = 0;
            var players = GameState.FootballPlayers;
            var ball = GameState.Ball;

            for (var i = 0; i < 22; i++)
                if (LastKicker == GameState.FootballPlayers[i])
                    lastTeam = i < 11 ? 1 : 2;

            // corner kicks or goal kicks (goal line crossed)
            if (ball.Position.Y > 75f / 2 + 7.32 / 2 || ball.Position.Y < 75f / 2 - 7.32 / 2) // not goals
            {
                if (GameState.Ball.Position.X > 110)
                {
                    if (WhoIsOnLeft == lastTeam)
                    {
                        var goalKeeper = lastTeam == 1 ? players[11] : players[0];
                        // goal kick
                        goalKeeper.Position.X = 110 - 16.5f;
                        goalKeeper.Position.Y = 75 / 2f;
                        goalKeeper.Movement.X = 0;
                        goalKeeper.Movement.Y = 0;

                        ball.Position.X = goalKeeper.Position.X;
                        ball.Position.Y = goalKeeper.Position.Y;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        if (lastTeam == 1)
                            Match.Shots1++;
                        else
                            Match.Shots2++;
                    }
                    else
                    {
                        // corner kick
                        ball.Position.X = 109;
                        ball.Position.Y = ball.Position.Y >= 75f / 2 + 7.32 / 2 ? 74 : 1;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        var nearestPlayerFromOppositeTeam =
                            GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                        nearestPlayerFromOppositeTeam.Movement.X = 0;
                        nearestPlayerFromOppositeTeam.Movement.Y = 0;
                        nearestPlayerFromOppositeTeam.Position.X = 110;
                        nearestPlayerFromOppositeTeam.Position.Y = ball.Position.Y < 75/2f ? 0 : 75;

                    }
                    // push all opponent players aways from the kickoff position
                    PushPlayersFromPosition(lastTeam, ball.Position);
                }

                if (GameState.Ball.Position.X < 0)
                {
                    if (WhoIsOnLeft != lastTeam)
                    {
                        var goalKeeper = lastTeam == 1 ? players[11] : players[0];

                        // goal kick
                        goalKeeper.Position.X = 16.5f;
                        goalKeeper.Position.Y = 75 / 2f;
                        goalKeeper.Movement.X = 0;
                        goalKeeper.Movement.Y = 0;

                        ball.Position.X = goalKeeper.Position.X;
                        ball.Position.Y = goalKeeper.Position.Y;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        if (lastTeam == 1)
                            Match.Shots1++;
                        else
                            Match.Shots2++;
                    }
                    else
                    {
                        // corner kick
                        ball.Position.X = 1;
                        ball.Position.Y = ball.Position.Y >= 75f / 2 + 7.32 / 2 ? 74 : 1;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        var nearestPlayerFromOppositeTeam =
                           GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                        nearestPlayerFromOppositeTeam.Position.X = 0;
                        nearestPlayerFromOppositeTeam.Position.Y = ball.Position.Y < 75/2f ? 0 : 75;
                        nearestPlayerFromOppositeTeam.Movement.X = 0;
                        nearestPlayerFromOppositeTeam.Movement.Y = 0;

                    }
                    // push all opponent players aways from the kickoff position
                    PushPlayersFromPosition(lastTeam, ball.Position);
                }
            }

            // touch lines
            if (ball.Position.Y < 0)
            {
                ball.Position.Y = 1;
                ball.Movement.X = 0f;
                ball.Movement.Y = 0f;

                var nearestPlayerFromOppositeTeam =
                          GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                nearestPlayerFromOppositeTeam.Position.X = ball.Position.X;
                nearestPlayerFromOppositeTeam.Position.Y = 0;
                nearestPlayerFromOppositeTeam.Movement.X = 0;
                nearestPlayerFromOppositeTeam.Movement.Y = 0;

                // push all opponent players aways from the kickoff position
                PushPlayersFromPosition(lastTeam, ball.Position);

            }
            if (ball.Position.Y > 75)
            {
                ball.Position.Y = 74;
                ball.Movement.X = 0f;
                ball.Movement.Y = 0f;

                var nearestPlayerFromOppositeTeam =
                    GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                nearestPlayerFromOppositeTeam.Position.X = ball.Position.X;
                nearestPlayerFromOppositeTeam.Position.Y = 75;
                nearestPlayerFromOppositeTeam.Movement.X = 0;
                nearestPlayerFromOppositeTeam.Movement.Y = 0;

                // push all opponent players aways from the kickoff position
                PushPlayersFromPosition(lastTeam, ball.Position);
            }

        }

        private void HandleGoals()
        {
            var lastTeam = 0;
            var ball = GameState.Ball;

            for (var i = 0; i < 22; i++)
                if (LastKicker == GameState.FootballPlayers[i])
                    lastTeam = i < 11 ? 1 : 2;

            if (ball.Position.X < 0 && ball.Position.Y < 75f / 2 + 7.32 / 2 && ball.Position.Y > 75f / 2 - 7.32 / 2)
            {
                var teamNameThatScored =
                    WhoIsOnLeft == 1 ? Player2Connection.PlayerName : Player1Connection.PlayerName;

                if (WhoIsOnLeft == 1)
                {
                    CurrentScore2++;
                    if (lastTeam == 2)
                    {
                        Match.ShotsOnTarget2++;
                        Match.Shots2++;
                    }
                }
                if (WhoIsOnLeft == 2)
                {
                    CurrentScore1++;
                    if (lastTeam == 1)
                    {
                        Match.ShotsOnTarget2++;
                        Match.Shots2++;
                    }
                }

                var scoringPlayerNumber = 0;
                for (var i = 0; i < 22; i++)
                    if (GameState.FootballPlayers[i] == LastKicker)
                        scoringPlayerNumber = i < 11 ? i : i - 11;
                Match.Goals += $"{CurrentTime};{teamNameThatScored};Player{scoringPlayerNumber}|";
                SetStartingPositions(WhoIsOnLeft);
            }
            else if (ball.Position.X > 110 && ball.Position.Y < 75f / 2 + 7.32 / 2 && ball.Position.Y > 75f / 2 - 7.32 / 2)
            {
                var teamNameThatScored =
                    WhoIsOnLeft == 2 ? Player2Connection.PlayerName : Player1Connection.PlayerName;

                if (WhoIsOnLeft == 1)
                {
                    CurrentScore1++;
                    if (lastTeam == 1)
                    {
                        Match.ShotsOnTarget1++;
                        Match.Shots1++;
                    }
                }
                if (WhoIsOnLeft == 2)
                {
                    CurrentScore2++;
                    if (lastTeam == 2)
                    {
                        Match.ShotsOnTarget2++;
                        Match.Shots2++;
                    }
                }

                var scoringPlayerNumber = 0;
                for (var i = 0; i < 22; i++)
                    if (GameState.FootballPlayers[i] == LastKicker)
                        scoringPlayerNumber = i < 11 ? i : i - 11;
                Match.Goals += $"{CurrentTime};{teamNameThatScored};Player{scoringPlayerNumber}|";
                SetStartingPositions(WhoIsOnLeft == 1 ? 2 : 1);
            }
        }

        private void UpdateStoppedShots(FootballPlayer currentWinner)
        {
            var ball = GameState.Ball;
            var currentWinnerTeam = 0;
            var lastWinnerTeam = 0;
            for (var i = 0; i < 11; i++)
            {
                if (GameState.FootballPlayers[i] == currentWinner)
                    currentWinnerTeam = i < 11 ? 1 : 2;
                if (GameState.FootballPlayers[i] == LastKicker)
                    lastWinnerTeam = i < 11 ? 1 : 2;
            }

            if (lastWinnerTeam == currentWinnerTeam)
                return;

            var intersectionWithGoalLine1 = GetIntersectionWithGoalLine(GameState.Ball, 1);
            var intersectionWithGoalLine2 = GetIntersectionWithGoalLine(GameState.Ball, 2);

            var wasBallGoingToGoalLine1 = intersectionWithGoalLine1 != null;
            var wasBallGoingToGoalLine2 = intersectionWithGoalLine2 != null;
            var wasBallGoingToGoalPost1 = intersectionWithGoalLine1 != null
                                          && intersectionWithGoalLine1.Y > 75f / 2 - 7.32 &&
                                          intersectionWithGoalLine1.Y < 75f / 2 + 7.32;
            var wasBallGoingToGoalPost2 = intersectionWithGoalLine2 != null
                                          && intersectionWithGoalLine2.Y > 75f / 2 - 7.32 &&
                                          intersectionWithGoalLine2.Y < 75f / 2 + 7.32;


            // process result
            if (wasBallGoingToGoalLine1 && lastWinnerTeam != WhoIsOnLeft)
            {
                if (lastWinnerTeam == 1)
                    Match.Shots1++;
                else
                    Match.Shots2++;
                if (wasBallGoingToGoalPost1)
                {
                    if (lastWinnerTeam == 1)
                        Match.ShotsOnTarget1++;
                    else
                        Match.ShotsOnTarget2++;
                }
            }

            if (wasBallGoingToGoalLine2 && lastWinnerTeam == WhoIsOnLeft)
            {
                if (lastWinnerTeam == 1)
                    Match.Shots1++;
                else
                    Match.Shots2++;

                if (wasBallGoingToGoalPost1)
                {
                    if (lastWinnerTeam == 1)
                        Match.ShotsOnTarget1++;
                    else
                        Match.ShotsOnTarget2++;
                }
            }


        }

        private void PushPlayersFromPosition(int teamToBePushedNumber, Vector position)
        {
            var toBePushedPlayers = new List<FootballPlayer>();

            for (var i = 0; i < 11; i++)
            {
                var player = GameState.FootballPlayers[teamToBePushedNumber == 1 ? i : i + 11];
                if (Vector.DistanceBetween(player.Position, position) < MinimalOpponentLengthFromCornerKick)
                    toBePushedPlayers.Add(player);
            }

            foreach (var player in toBePushedPlayers)
            {
                var vectorToPlayer = new Vector(player.Position.X - position.X, player.Position.Y - position.Y);
                var length = vectorToPlayer.Length;

                // apply push
                vectorToPlayer.X *= MinimalOpponentLengthFromCornerKick/length;
                vectorToPlayer.Y *= MinimalOpponentLengthFromCornerKick/length;

                player.Position.X = position.X + vectorToPlayer.X;
                player.Position.Y = position.Y + vectorToPlayer.Y;

                if (player.Position.X > 110)
                    player.Position.X = 110 - MinimalOpponentLengthFromCornerKick;
                if (player.Position.Y > 75)
                    player.Position.Y = 75 - MinimalOpponentLengthFromCornerKick;
                if (player.Position.X < 0)
                    player.Position.X = MinimalOpponentLengthFromCornerKick;
                if (player.Position.Y < 0)
                    player.Position.Y = MinimalOpponentLengthFromCornerKick;


            }

        }

        private FootballPlayer GetNearestPlayerToBall(int fromWhichTeam)
        {
            FootballPlayer nearestPlayer = null;
            var players = GameState.FootballPlayers;
            var ball = GameState.Ball;

            if (fromWhichTeam == 1)
            {
                nearestPlayer = players[1];
                for (int i = 2; i < 11; i++)
                    if (Vector.DistanceBetween(ball.Position, players[i].Position) < 
                        Vector.DistanceBetween(ball.Position, nearestPlayer.Position))
                        nearestPlayer = players[i];
            }
            if (fromWhichTeam == 2)
            {
                nearestPlayer = players[12];
                for (int i = 13; i < 22; i++)
                    if (Vector.DistanceBetween(ball.Position, players[i].Position) < 
                        Vector.DistanceBetween(ball.Position, nearestPlayer.Position))
                        nearestPlayer = players[i];
            }

            return nearestPlayer;
        }

        private Vector GetIntersectionWithGoalLine(Ball ball, int whichGoalLine)
        {
            double t = 0;
            if (ball.Movement.X == 0)
                return null;

            if (whichGoalLine == 1)
                t = -ball.Position.X / ball.Movement.X;
            else
                t = (110 - ball.Position.X) / ball.Movement.X;

            if (t < 0)
                return null;

            var intersection = new Vector
            {
                X = ball.Position.X + t * ball.Movement.X,
                Y = ball.Position.Y + t * ball.Movement.Y
            };

            // calculate speed at intersection
            var distanceFromIntersection =
                Math.Sqrt(Math.Pow(intersection.X - ball.Position.X, 2) - Math.Pow(intersection.Y - ball.Position.Y, 2));
            
            // at^2 + 2(v_0)t -s = 0, where v_0 = start ball speed, a = acceleration, s = distance, t = time
            // from that equation we calculate t
            var discriminant = 4*Math.Pow(ball.CurrentSpeed, 2) - 8*BallDecerelation*distanceFromIntersection;
            if (discriminant < 0)
                return null; // ball will stop -> no intersection reached

            var time = (ball.CurrentSpeed - Math.Sqrt(discriminant))/BallDecerelation;

            // final speed = v + a * t
            var speedAtIntersection = ball.CurrentSpeed + BallDecerelation*time;

            // is speed higher than minimal shot speed (4 for now)
            if (speedAtIntersection <= 4)
                return null;

            return intersection;
        }

        private FootballPlayer GetKickWinner(FootballPlayer[] kickers)
        {
            if (kickers.Length == 0)
                return null;
            if (LastKicker != null && kickers.Contains(LastKicker))
            {
                kickers = kickers.Where(k => k != LastKicker).ToArray();
                var lastKickerWeight = 1.5 + LastKicker.Possesion;
                var winNumber = Random.NextDouble() * (kickers.Length + lastKickerWeight);
                winNumber -= lastKickerWeight;
                if (winNumber < 0)
                    return LastKicker;
                var intWinner = (int)Math.Floor(winNumber);
                return kickers[intWinner];
            }
            else
            {
                var winNumber = Random.Next(0, kickers.Length);
                return kickers[winNumber];
            }
        }

    }
}
