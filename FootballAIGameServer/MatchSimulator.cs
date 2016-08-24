using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
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
        public const int StepInterval = 200;

        public const double MaxAcceleration = 3; // m/s/s
        public const double MaxBallSpeed = 15; // m/s
        public const double BallDecerelation = 1.5; // m/s/s

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

        public static List<MatchSimulator> RunningSimulations { get; set; }
        public static Random Random { get; set; }

        public MatchSimulator(ClientConnection player1Connection, ClientConnection player2Connection)
        {
            this.Player1Connection = player1Connection;
            this.Player2Connection = player2Connection;
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
                Player2Ai = Player2Connection.AiName,
                Goals = ""
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
            players[4].Y = 60;

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
            if ((whoHasBall == 1 && WhoIsOnLeft == 1) || (whoHasBall == 2 && WhoIsOnLeft == 2))
            {
                players[10].X = 54;
                players[10].Y = 75 / 2f;
            }
            else
            {
                players[21].X = 56;
                players[21].Y = 75 / 2f;
            }

            if (WhoIsOnLeft == 2)
            {
                // switch
                for (var i = 0; i < 11; i++)
                {
                    var tempX = players[i].X;
                    var tempY = players[i].Y;
                    players[i].X = players[i + 11].X;
                    players[i].Y = players[i + 11].Y;
                    players[i + 11].X = tempX;
                    players[i + 11].Y = tempY;
                }
            }
        }

        public async Task ProcessSimulationStep(int step)
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
                return;
            }


            UpdateMatch(actionMessage1, actionMessage2);
            SaveState();
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
                Match.Score = $"{CurrentScore1}:{CurrentScore2}";
                Match.Winner = CurrentScore1 > CurrentScore2 ? 1 : CurrentScore1 < CurrentScore2 ? 2 : 0;

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
            var currentStateData = new float[46];
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

        private void UpdateMatch(ActionMessage player1Action, ActionMessage player2Action)
        {
            UpdatePlayersMovement(player1Action, player2Action);
            UpdateBall(player1Action, player2Action);
            HandleOut();
            HandleGoals();
        }

        private void HandleOut()
        {
            var lastTeam = 0;
            var players = GameState.FootballPlayers;
            var ball = GameState.Ball;

            for (var i = 0; i < 22; i++)
                if (LastKicker == GameState.FootballPlayers[i])
                    lastTeam = i < 11 ? 1 : 2;

            // corner kicks or goal kicks (goal line crossed)
            if (ball.Y > 75f / 2 + 7.32 / 2 || ball.Y < 75f / 2 - 7.32 / 2) // not goals
            {
                if (GameState.Ball.X > 110)
                {
                    if (WhoIsOnLeft == lastTeam)
                    {
                        var goalKeeper = lastTeam == 1 ? players[0] : players[11];
                        // goal kick
                        goalKeeper.X = 110 - 16.5f;
                        goalKeeper.Y = 75 / 2f;
                        ball.X = goalKeeper.X;
                        ball.Y = goalKeeper.Y;

                        if (lastTeam == 1)
                            Match.Shots1++;
                        else
                            Match.Shots2++;
                        // todo kick enemy players from penalty area
                    }
                    else
                    {
                        // corner kick
                        ball.X = 110;
                        ball.Y = ball.Y >= 75f / 2 + 7.32 / 2 ? 75 : 0;

                        var nearestPlayerFromOppositeTeam =
                            GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                        nearestPlayerFromOppositeTeam.X = 111;
                        nearestPlayerFromOppositeTeam.Y = ball.Y == 0 ? -1 : 76;
                    }
                }

                if (GameState.Ball.X < 0)
                {
                    if (WhoIsOnLeft != lastTeam)
                    {
                        var goalKeeper = lastTeam == 1 ? players[0] : players[11];
                        // goal kick
                        goalKeeper.X = 16.5f;
                        goalKeeper.Y = 75 / 2f;
                        ball.X = goalKeeper.X;
                        ball.Y = goalKeeper.Y;
                        // todo kick enemy players from penalty area

                        if (lastTeam == 1)
                            Match.Shots1++;
                        else
                            Match.Shots2++;
                    }
                    else
                    {
                        // corner kick
                        ball.X = 0;
                        ball.Y = ball.Y >= 75f / 2 + 7.32 / 2 ? 75 : 0;

                        var nearestPlayerFromOppositeTeam =
                           GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                        nearestPlayerFromOppositeTeam.X = -1;
                        nearestPlayerFromOppositeTeam.Y = ball.Y == 0 ? -1 : 76;
                    }
                }
            }

            // touch lines
            if (ball.Y < 0)
            {
                ball.Y = 0;

                var nearestPlayerFromOppositeTeam =
                          GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                nearestPlayerFromOppositeTeam.X = ball.X;
                nearestPlayerFromOppositeTeam.Y = -1;
            }
            if (ball.Y > 75)
            {
                ball.Y = 75;

                var nearestPlayerFromOppositeTeam =
                    GetNearestPlayerToBall(lastTeam == 1 ? 2 : 1);

                nearestPlayerFromOppositeTeam.X = ball.X;
                nearestPlayerFromOppositeTeam.Y = 76;
            }

        }

        private void HandleGoals()
        {
            var lastTeam = 0;
            var players = GameState.FootballPlayers;
            var ball = GameState.Ball;

            for (var i = 0; i < 22; i++)
                if (LastKicker == GameState.FootballPlayers[i])
                    lastTeam = i < 11 ? 1 : 2;

            if (ball.X < 0 && ball.Y < 75f / 2 + 7.32 / 2 && ball.Y > 75f / 2 - 7.32 / 2)
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
            else if (ball.X > 110 && ball.Y < 75f / 2 + 7.32 / 2 && ball.Y > 75f / 2 - 7.32 / 2)
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

        private FootballPlayer GetNearestPlayerToBall(int fromWhichTeam)
        {
            FootballPlayer nearestPlayer = null;
            FootballPlayer[] players = GameState.FootballPlayers;
            Ball ball = GameState.Ball;

            if (fromWhichTeam == 1)
            {
                nearestPlayer = players[1];
                for (int i = 2; i < 11; i++)
                    if (DistanceBetween(ball, players[i]) < DistanceBetween(ball, nearestPlayer))
                        nearestPlayer = players[i];
            }
            if (fromWhichTeam == 2)
            {
                nearestPlayer = players[12];
                for (int i = 13; i < 22; i++)
                    if (DistanceBetween(ball, players[i]) < DistanceBetween(ball, nearestPlayer))
                        nearestPlayer = players[i];
            }

            return nearestPlayer;
        }

        private void UpdateBall(ActionMessage player1Action, ActionMessage player2Action)
        {
            // add kick actions to players
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i].KickX = player1Action?.PlayerActions[i].KickX ?? 0;
                GameState.FootballPlayers[i].KickY = player1Action?.PlayerActions[i].KickY ?? 0;
            }
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i + 11].KickX = player2Action?.PlayerActions[i].KickX ?? 0;
                GameState.FootballPlayers[i + 11].KickY = player2Action?.PlayerActions[i].KickY ?? 0;
            }

            var playersNearBallKicking = GameState.FootballPlayers.Where(
                p => DistanceBetween(GameState.Ball, p) <= 2 && (p.KickX != 0 || p.KickY != 0));

            var kickWinner = GetKickWinner(playersNearBallKicking.ToArray());

            // apply kick
            if (kickWinner != null)
            {

                // check whether the ball was shot or shot on target
                UpdateStoppedShots(kickWinner);

                LastKicker = kickWinner;
                GameState.Ball.VectorX = kickWinner.KickX;
                GameState.Ball.VectorY = kickWinner.KickY;

                // deviation of the kick
                var angleDevation = (0.4 - kickWinner.Precision) * (Random.NextDouble() * 2 - 1);

                // rotation applied (deviation)
                GameState.Ball.VectorX = (float)(Math.Cos(angleDevation) * GameState.Ball.VectorX -
                                                Math.Sin(angleDevation) * GameState.Ball.VectorY);
                GameState.Ball.VectorY = (float)(Math.Sin(angleDevation) * GameState.Ball.VectorX +
                                                Math.Cos(angleDevation) * GameState.Ball.VectorY);

                var newSpeed = GameState.Ball.CurrentSpeed;
                var maxAllowedSpeed = 15 + LastKicker.KickPower * 10;
                if (newSpeed > maxAllowedSpeed)
                {
                    GameState.Ball.VectorX *= (float)(maxAllowedSpeed / newSpeed);
                    GameState.Ball.VectorY *= (float)(maxAllowedSpeed / newSpeed);
                }
            }

            // ball deceralation
            var ratio = (GameState.Ball.CurrentSpeed - (BallDecerelation * StepInterval / 1000)) /
                GameState.Ball.CurrentSpeed;
            if (ratio < 0)
                ratio = 0;
            GameState.Ball.VectorX *= (float)ratio;
            GameState.Ball.VectorY *= (float)ratio;

            // update ball position
            GameState.Ball.X += GameState.Ball.VectorX;
            GameState.Ball.Y += GameState.Ball.VectorY;
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

            var wasBallGoingToGoalLine1 = false;
            var wasBallGoingToGoalLine2 = false;
            var wasBallGoingToGoalPost2 = false;
            var wasBallGoingToGoalPost1 = false;

            #region calculations

            var vectorToLowerEdgeX = 0 - ball.X;
            var vectorToHigherEdgeX = 0 - ball.X;
            var vectorToHigherEdgeY = 0.0;
            var vectorToLowerEdgeY = 0.0;

            Func<bool> condition = () =>
                    ball.VectorX*vectorToHigherEdgeX + ball.VectorY*vectorToHigherEdgeY < 0 &&
                   ball.VectorX*vectorToLowerEdgeX + ball.VectorY*vectorToLowerEdgeY > 0;


            if (condition())
                wasBallGoingToGoalLine1 = true;

            vectorToLowerEdgeY = 75 / 2f + 7.32 / 2 - ball.Y;
            vectorToHigherEdgeY = 75 / 2f - 7.32 / 2 - ball.Y;

            if (condition())
                wasBallGoingToGoalPost1 = true;

            vectorToLowerEdgeX = 110 - ball.X;
            vectorToHigherEdgeX = 110 - ball.X;
            vectorToHigherEdgeY = 0.0;
            vectorToLowerEdgeY = 0.0;

            if (condition())
                wasBallGoingToGoalLine2 = true;

            vectorToLowerEdgeY = 75 / 2f + 7.32 / 2 - ball.Y;
            vectorToHigherEdgeY = 75 / 2f - 7.32 / 2 - ball.Y;

            if (condition())
                wasBallGoingToGoalPost2 = true;
#endregion

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

        private Point GetIntersectionWithGoalLine(Ball ball, int whichGoalLine)
        {
            double t = 0;

            if (whichGoalLine == 1)
                t = -ball.X / ball.VectorX;
            else
                t = (110 - ball.X) / ball.VectorX;

            return new Point
            {
                X = ball.X + t * ball.VectorX,
                Y = ball.Y + t * ball.VectorY
            };

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

        private double DistanceBetween(Ball ball, FootballPlayer player)
        {
            return Math.Sqrt(Math.Pow(ball.X - player.X, 2) + Math.Pow(ball.Y - player.Y, 2));
        }

        public void UpdatePlayersMovement(ActionMessage player1Action, ActionMessage player2Action)
        {
            if (player1Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i];
                    var action = player1Action.PlayerActions[i];

                    var oldSpeed = player.CurrentSpeed;

                    var accelerationX = action.VectorX - player.VectorX;
                    var accelerationY = action.VectorY - player.VectorY;

                    var accelerationVectorLength =
                        Math.Sqrt(Math.Pow(accelerationX, 2) *
                                  Math.Pow(accelerationY, 2));


                    if (accelerationVectorLength > MaxAcceleration)
                    {
                        var q = MaxAcceleration / accelerationVectorLength;
                        var fixedAccelerationX = accelerationX * q;
                        var fixedAccelerationY = accelerationY * q;

                        //Match.Player1ErrorLog += "Player acceleration correction.;";
                        action.VectorX = (float)(player.VectorX + fixedAccelerationX);
                        action.VectorY = (float)(player.VectorY + fixedAccelerationY);
                    }


                    player.VectorX = action.VectorX;
                    player.VectorY = action.VectorY;
                    var newSpeed = player.CurrentSpeed;

                    if (newSpeed > player.MaxSpeed)
                    {
                        // too high speed
                        //Match.Player1ErrorLog += "Player speed correction.";
                        player.VectorX *= (float)(player.MaxSpeed / newSpeed);
                        player.VectorY *= (float)(player.MaxSpeed / newSpeed);
                    }

                    // apply
                    player.X += player.VectorX;
                    player.Y += player.VectorY;

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

                    var oldSpeed = player.CurrentSpeed;

                    var accelerationX = action.VectorX - player.VectorX;
                    var accelerationY = action.VectorY - player.VectorY;

                    var accelerationVectorLength =
                        Math.Sqrt(Math.Pow(accelerationX, 2) *
                                  Math.Pow(accelerationY, 2));


                    if (accelerationVectorLength > MaxAcceleration)
                    {
                        var q = MaxAcceleration / accelerationVectorLength;
                        var fixedAccelerationX = accelerationX * q;
                        var fixedAccelerationY = accelerationY * q;

                        //Match.Player2ErrorLog += "Player acceleration correction.;";
                        action.VectorX = (float)(player.VectorX + fixedAccelerationX);
                        action.VectorY = (float)(player.VectorY + fixedAccelerationY);
                    }


                    player.VectorX = action.VectorX;
                    player.VectorY = action.VectorY;
                    var newSpeed = player.CurrentSpeed;

                    if (newSpeed > player.MaxSpeed)
                    {
                        // too high speed
                        //Match.Player2ErrorLog += "Player speed correction.";
                        player.VectorX *= (float)(player.MaxSpeed / newSpeed);
                        player.VectorY *= (float)(player.MaxSpeed / newSpeed);
                    }

                    // apply
                    player.X += player.VectorX;
                    player.Y += player.VectorY;

                }
            }
            else
            {
                // default (nothing for now)
                Console.WriteLine(Player2Connection.PlayerName + " timeout.");
            }
        }

    }

    struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
