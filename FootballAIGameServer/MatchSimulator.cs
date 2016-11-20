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
    /// <summary>
    /// Responsible for simulating a football match between two AIs.
    /// </summary>
    public class MatchSimulator
    {
        #region Simulation constants

        /// <summary>
        /// The total number of simulation steps.
        /// </summary>
        public const int NumberOfSimulationSteps = 1500;

        /// <summary>
        /// The time in milliseconds that player AI has for generating action in one simulation step.
        /// </summary>
        public const int PlayerTimeForOneStep = 100; // [ms]

        /// <summary>
        /// The time in milliseconds of one simulation step.
        /// </summary>
        public const int StepInterval = 200; // [ms]

        /// <summary>
        /// The maximum allowed acceleration in meters per second squared of football player.
        /// </summary>
        public const double MaxAcceleration = 3; // [m/s/s]

        /// <summary>
        /// The maximum ball speed in meters per second.
        /// </summary>
        public const double MaxBallSpeed = 15; // [m/s]

        /// <summary>
        /// The ball deceleration in meters per second squared.
        /// </summary>
        public const double BallDecerelation = 1.5; // [m/s/s]

        /// <summary>
        /// The minimal opponent length from kickoff in meters.
        /// </summary>
        public const double MinimalOpponentDirectionFromKickoff = 6; // [m]

        /// <summary>
        /// The maximum number of errors of the same kind in the error log.
        /// </summary>
        public const int MaximumNumberOfSameKindErrorInLog = 5;

        #endregion

        /// <summary>
        /// Gets or sets the current simulation task.
        /// </summary>
        /// <value>
        /// The current simulation task.
        /// </value>
        public Task CurrentSimulationTask { get; private set; }

        /// <summary>
        /// Gets the first player AI <see cref="ClientConnection"/>.
        /// </summary>
        /// <value>
        /// The player1 AI connection.
        /// </value>
        public ClientConnection Player1AiConnection { get; private set; }

        /// <summary>
        /// Gets the second player AI <see cref="ClientConnection"/>.
        /// </summary>
        /// <value>
        /// The player2 AI connection.
        /// </value>
        public ClientConnection Player2AiConnection { get; private set; }

        /// <summary>
        /// Gets or sets the current simulation step.
        /// </summary>
        /// <value>
        /// The current simulation step.
        /// </value>
        public int CurrentStep { get; set; }

        /// <summary>
        /// Gets or sets the winner. If the match has not yet ended or it ended
        /// in draw, returns null.
        /// </summary>
        public string Winner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether first player has requested to cancel the match.
        /// </summary>
        /// <value>
        /// <c>true</c> if first player requested to cancel the match; otherwise, <c>false</c>.
        /// </value>
        public bool Player1CancelRequested { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether second player has requested to cancel the match.
        /// </summary>
        /// <value>
        /// <c>true</c> if second player requested to cancel the match; otherwise, <c>false</c>.
        /// </value>
        public bool Player2CancelRequested { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Random"/> used for generating random numbers.
        /// </summary>
        public static Random Random { get; set; }

        private GameState GameState { get; set; }
        private Match Match { get; set; }
        private int Ping1 { get; set; }
        private int Ping2 { get; set; }
        private List<float> MatchData { get; set; }
        private Task<ClientMessage> Message1 { get; set; }
        private Task<ClientMessage> Message2 { get; set; }
        private FootballPlayer LastKicker { get; set; }
        private int WhoIsOnLeft { get; set; }
        
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
        private int CurrentScore1 { get; set; }
        private int CurrentScore2 { get; set; }
        private int NumberOfAccelerationCorrections1 { get; set; }
        private int NumberOfAccelerationCorrections2 { get; set; }
        private int NumberOfSpeedCorrections1 { get; set; }
        private int NumberOfSpeedCorrections2 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchSimulator"/> class.
        /// </summary>
        /// <param name="player1AiConnection">The player1 AI connection that will play the match.</param>
        /// <param name="player2AiConnection">The player2 AI connection that will play the match.</param>
        public MatchSimulator(ClientConnection player1AiConnection, ClientConnection player2AiConnection)
        {
            this.Player1AiConnection = player1AiConnection;
            this.Player2AiConnection = player2AiConnection;
        }

        /// <summary>
        /// Simulates the match.
        /// </summary>
        /// <returns>Task that will be completed with simulation.</returns>
        public async Task SimulateMatch()
        {
            CurrentSimulationTask = Simulate();
            await CurrentSimulationTask;
        }

        /// <summary>
        /// Simulates the match. <see cref="SimulateMatch"/> is it's public wrapper, that is used to set 
        /// <see cref="CurrentSimulationTask"/> property.
        /// </summary>
        private async Task Simulate()
        {
            await ProcessSimulationStart();

            var firstBall = Random.Next(1, 3); // max is excluded

            try
            {
                // first half
                WhoIsOnLeft = 1;
                SetStartingPositions(firstBall);
                SaveState(); // save starting state

                for (var step = 0; step < NumberOfSimulationSteps / 2; step++)
                {
                    if (Player1CancelRequested || Player2CancelRequested || !Player1AiConnection.IsActive ||
                        !Player2AiConnection.IsActive)
                    {
                        ProcessSimulationEnd();
                        return;
                    }

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
                    if (Player1CancelRequested || Player2CancelRequested || !Player1AiConnection.IsActive ||
                        !Player2AiConnection.IsActive)
                    {
                        ProcessSimulationEnd();
                        return;
                    }

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

        /// <summary>
        /// Processes the simulation start. New match is initialized. Players parameters
        /// and ping are set.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessSimulationStart()
        {
            Player1AiConnection.IsInMatch = true;
            Player2AiConnection.IsInMatch = true;

            Console.WriteLine($"Starting simulation between {Player1AiConnection.PlayerName}:{Player1AiConnection.AiName} " +
                              $"and {Player2AiConnection.PlayerName}:{Player2AiConnection.AiName}");

            Match = new Match()
            {
                Time = DateTime.Now,
                Player1Ai = Player1AiConnection.AiName,
                Player2Ai = Player2AiConnection.AiName,
                Player1ErrorLog = "",
                Player2ErrorLog = "",
                Goals = ""
            };

            // 1. check pings
            Ping1 = Player1AiConnection.PingTimeAverage();
            Ping2 = Player2AiConnection.PingTimeAverage();
            //Console.WriteLine($"{Player1Connection.PlayerName} ping = {Ping1}\n" +
            //                  $"{Player2Connection.PlayerName} ping = {Ping2}");

            MatchData = new List<float>();

            GameState = new GameState
            {
                Ball = new Ball(),
                FootballPlayers = new FootballPlayer[22]
            };

            await ProcessGettingParameters();

            Message1 = Player1AiConnection.ReceiveClientMessageAsync();
            Message2 = Player2AiConnection.ReceiveClientMessageAsync();
        }

        /// <summary>
        /// Processes the simulation step. Client action are retrieved and the game state is updated
        /// accordingly.
        /// </summary>
        /// <param name="step">The step number.</param>
        /// <returns></returns>
        private async Task ProcessSimulationStep(int step)
        {
            ActionMessage actionMessage1 = null;
            ActionMessage actionMessage2 = null;
            GameState.Step = step;

            try
            {
                await Player1AiConnection.SendAsync("GET ACTION");
                await Player1AiConnection.SendAsync(GameState, 1);

                await Player2AiConnection.SendAsync("GET ACTION");
                await Player2AiConnection.SendAsync(GameState, 2);

                var receiveActionMessage1 = Player1AiConnection.ReceiveActionMessageAsync(step);
                var receiveActionMessage2 = Player2AiConnection.ReceiveActionMessageAsync(step);

                var getMessage1Task = Task.WhenAny(receiveActionMessage1, Task.Delay(Ping1 + PlayerTimeForOneStep));
                var getMessage2Task = Task.WhenAny(receiveActionMessage2, Task.Delay(Ping2 + PlayerTimeForOneStep));

                var getMessage1Result = await getMessage1Task;
                var getMessage2Result = await getMessage2Task;

                if (Message1.IsFaulted || !Player1AiConnection.IsActive || Message2.IsFaulted ||
                    !Player2AiConnection.IsActive)
                {
                    return;
                }

                if (getMessage1Result == receiveActionMessage1 && !receiveActionMessage1.IsFaulted)
                {
                    actionMessage1 = Message1.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                    {
                        Message1 = Player1AiConnection.ReceiveClientMessageAsync();
                    }
                }

                if (getMessage2Result == receiveActionMessage2 && !receiveActionMessage2.IsFaulted)
                {
                    actionMessage2 = Message2.Result as ActionMessage;
                    if (step < NumberOfSimulationSteps - 1)
                    {
                        Message2 = Player2AiConnection.ReceiveClientMessageAsync();
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

        /// <summary>
        /// Processes the simulation end. The match is saved to the database.
        /// </summary>
        private void ProcessSimulationEnd()
        {
            Console.WriteLine($"Ending simulation between {Player1AiConnection.PlayerName}:{Player1AiConnection.AiName} " +
                             $"and {Player2AiConnection.PlayerName}:{Player2AiConnection.AiName}");
            using (var context = new ApplicationDbContext())
            {
                var player1 = context.Players.Single(p => p.Name == Player1AiConnection.PlayerName);
                var player2 = context.Players.Single(p => p.Name == Player2AiConnection.PlayerName);
                player1.PlayerState = PlayerState.Idle;
                player2.PlayerState = PlayerState.Idle;
                Match.Player1 = player1;
                Match.Player2 = player2;
                Match.Score = $"{CurrentScore1}:{CurrentScore2}";
                Match.Winner = CurrentScore1 > CurrentScore2 ? 1 : CurrentScore1 < CurrentScore2 ? 2 : 0;

                if (Message1.IsFaulted || !Player1AiConnection.IsActive)
                {
                    Match.Winner = 2;
                    Match.Player1ErrorLog += $"{CurrentTime} - Player AI has disconnected.;";
                }
                if (Message2.IsFaulted || !Player2AiConnection.IsActive)
                {
                    Match.Winner = 1;
                    Match.Player2ErrorLog += $"{CurrentTime} - Player AI has disconnected.;";
                }

                if (Player1CancelRequested)
                {
                    Match.Winner = 2;
                    Match.Player1ErrorLog += $"{CurrentTime} - Player has canceled the match.;";
                }
                if (Player2CancelRequested)
                {
                    Match.Winner = 1;
                    Match.Player2ErrorLog += $"{CurrentTime} - Player has canceled the match.;";
                }

                switch (Match.Winner)
                {
                    case 1:
                        player1.WonGames++;
                        Winner = Player1AiConnection.PlayerName;
                        break;
                    case 2:
                        player2.WonGames++;
                        Winner = Player2AiConnection.PlayerName;
                        break;
                }

                var matchByteData = new byte[MatchData.Count * 4];
                Buffer.BlockCopy(MatchData.ToArray(), 0, matchByteData, 0, MatchData.Count * 4);
                Match.MatchData = matchByteData;

                context.Matches.Add(Match);

                context.SaveChanges();

                SimulationManager.RunningSimulations.Remove(this);
                Player1AiConnection.IsInMatch = false;
                Player2AiConnection.IsInMatch = false;
            }
        }

        /// <summary>
        /// Processes getting of the players parameters from both AIs.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessGettingParameters()
        {
            var getParameters1 = GetPlayerParameters(Player1AiConnection);
            var getParameters2 = GetPlayerParameters(Player2AiConnection);
            var result1 = await Task.WhenAny(getParameters1, Task.Delay(500 + Ping1));
            var result2 = await Task.WhenAny(getParameters2, Task.Delay(500 + Ping2));

            // player 1
            if (result1 == getParameters1 && !getParameters1.IsFaulted)
            {
                for (var i = 0; i < 11; i++)
                {
                    var par = getParameters1.Result[i];
                    if (par.Speed <= 0.4001 && par.KickPower <= 0.4001 &&
                        par.Possession <= 0.4001 && par.Precision <= 0.4001 &&
                        par.Speed + par.KickPower + par.Possession + par.Precision - 1 <= 0.01)
                    {
                        GameState.FootballPlayers[i] = getParameters1.Result[i];
                    }
                    else // default parameters
                    {
                        GameState.FootballPlayers[i] = new FootballPlayer()
                        {
                            Speed = 0.25f,
                            KickPower = 0.25f,
                            Possession = 0.25f,
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
                        Possession = 0.25f,
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
                        par.Possession <= 0.4001 && par.Precision <= 0.4001 &&
                        par.Speed + par.KickPower + par.Possession + par.Precision - 1 <= 0.01)
                    {
                        GameState.FootballPlayers[i + 11] = getParameters2.Result[i];
                    }
                    else // default parameters
                    {
                        GameState.FootballPlayers[i + 11] = new FootballPlayer()
                        {
                            Speed = 0.25f,
                            KickPower = 0.25f,
                            Possession = 0.25f,
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
                        Possession = 0.25f,
                        Precision = 0.25f
                    };
            }
        }

        /// <summary>
        /// Sets the starting positions.
        /// </summary>
        /// <param name="whoHasBall">The number indicating whether player1 (1) or player2 (2) has the ball.</param>
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

            // no one is moving
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

        /// <summary>
        /// Updates the match. Simulates current simulation step with the given players actions.
        /// </summary>
        /// <param name="player1Action">The player1 action.</param>
        /// <param name="player2Action">The player2 action.</param>
        private void UpdateMatch(ActionMessage player1Action, ActionMessage player2Action)
        {
            UpdatePlayers(player1Action, player2Action);
            UpdateBall(player1Action, player2Action);
            HandleOuts();
            HandleGoals();
        }

        /// <summary>
        /// Saves the current <see cref="GameState"/> to the <see cref="MatchData"/>.
        /// </summary>
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
                MatchData.Add(num);
        }

        /// <summary>
        /// Gets the player parameters from the given AI.
        /// </summary>
        /// <param name="playerConnection">The player AI connection.</param>
        /// <returns>The array of football players with their parameters set accordingly.</returns>
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

        /// <summary>
        /// Updates the players movements according to the given AI actions.
        /// </summary>
        /// <param name="player1Action">The player1 AI action.</param>
        /// <param name="player2Action">The player2 AI action.</param>
        private void UpdatePlayers(ActionMessage player1Action, ActionMessage player2Action)
        {
            if (player1Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i];
                    var action = player1Action.PlayersActions[i];

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
                Console.WriteLine(Player1AiConnection.PlayerName + " timeout.");
            }

            if (player2Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i + 11];
                    var action = player2Action.PlayersActions[i];

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
                Console.WriteLine(Player2AiConnection.PlayerName + " timeout.");
            }
        }

        /// <summary>
        /// Updates the ball movement according to the given AI actions.
        /// </summary>
        /// <param name="player1Action">The player1 AI action.</param>
        /// <param name="player2Action">The player2 AI action.</param>
        private void UpdateBall(ActionMessage player1Action, ActionMessage player2Action)
        {
            // add kick actions to players
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i].Kick.X = player1Action?.PlayersActions[i].Kick.X ?? 0;
                GameState.FootballPlayers[i].Kick.Y = player1Action?.PlayersActions[i].Kick.Y ?? 0;
            }
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i + 11].Kick.X = player2Action?.PlayersActions[i].Kick.X ?? 0;
                GameState.FootballPlayers[i + 11].Kick.Y = player2Action?.PlayersActions[i].Kick.Y ?? 0;
            }

            var playersNearBallKicking = GameState.FootballPlayers.Where(
                p => Vector.DistanceBetween(GameState.Ball.Position, p.Position) <= 2 && (p.Kick.X != 0 || p.Kick.Y != 0));

            var kickWinner = GetKickWinner(playersNearBallKicking.ToArray());

            // apply kick
            if (kickWinner != null)
            {
                // check whether the ball was shot or shot on target
                HandleStoppedShots(kickWinner);

                LastKicker = kickWinner;
                GameState.Ball.Movement.X = kickWinner.Kick.X;
                GameState.Ball.Movement.Y = kickWinner.Kick.Y;

                // deviation of the kick
                var angleDevation = 1 / 4f * (0.4 - kickWinner.Precision) * (Random.NextDouble() * 2 - 1);

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

            // ball deceleration
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

        /// <summary>
        /// Handles out of play rules simulation.
        /// </summary>
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
                        nearestPlayerFromOppositeTeam.Position.Y = ball.Position.Y < 75 / 2f ? 0 : 75;

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
                        nearestPlayerFromOppositeTeam.Position.Y = ball.Position.Y < 75 / 2f ? 0 : 75;
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

        /// <summary>
        /// Handles the goals.
        /// </summary>
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
                    WhoIsOnLeft == 1 ? Player2AiConnection.PlayerName : Player1AiConnection.PlayerName;

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
                    WhoIsOnLeft == 2 ? Player2AiConnection.PlayerName : Player1AiConnection.PlayerName;

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

        /// <summary>
        /// Handles the stopped shots. Updates shots and shots on target statistics
        /// accordingly.
        /// </summary>
        /// <param name="currentKickWinner">The current kick winner.</param>
        private void HandleStoppedShots(FootballPlayer currentKickWinner)
        {
            var ball = GameState.Ball;
            var currentWinnerTeam = 0;
            var lastWinnerTeam = 0;
            for (var i = 0; i < 11; i++)
            {
                if (GameState.FootballPlayers[i] == currentKickWinner)
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

        /// <summary>
        /// Pushes the players from the given team away from the given <see cref="position"/> in 
        /// accordance to the <see cref="MinimalOpponentDirectionFromKickoff"/>.
        /// Used for pushing opponent players from kickoffs.
        /// </summary>
        /// <param name="teamToBePushedNumber">The team to be pushed number.</param>
        /// <param name="position">The position from which the players will be pushed aways.</param>
        private void PushPlayersFromPosition(int teamToBePushedNumber, Vector position)
        {
            var toBePushedPlayers = new List<FootballPlayer>();

            for (var i = 0; i < 11; i++)
            {
                var player = GameState.FootballPlayers[teamToBePushedNumber == 1 ? i : i + 11];
                if (Vector.DistanceBetween(player.Position, position) < MinimalOpponentDirectionFromKickoff)
                    toBePushedPlayers.Add(player);
            }

            foreach (var player in toBePushedPlayers)
            {
                var vectorToPlayer = new Vector(player.Position.X - position.X, player.Position.Y - position.Y);
                var length = vectorToPlayer.Length;

                // apply push
                vectorToPlayer.X *= MinimalOpponentDirectionFromKickoff / length;
                vectorToPlayer.Y *= MinimalOpponentDirectionFromKickoff / length;

                player.Position.X = position.X + vectorToPlayer.X;
                player.Position.Y = position.Y + vectorToPlayer.Y;

                if (player.Position.X > 110)
                    player.Position.X = 110 - MinimalOpponentDirectionFromKickoff;
                if (player.Position.Y > 75)
                    player.Position.Y = 75 - MinimalOpponentDirectionFromKickoff;
                if (player.Position.X < 0)
                    player.Position.X = MinimalOpponentDirectionFromKickoff;
                if (player.Position.Y < 0)
                    player.Position.Y = MinimalOpponentDirectionFromKickoff;


            }

        }

        /// <summary>
        /// Gets the nearest player to the ball from the given team .
        /// </summary>
        /// <param name="fromWhichTeam">Team number.</param>
        /// <returns>Nearest player to the ball from the given team.</returns>
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

        /// <summary>
        /// Gets the intersection with goal line that ball would have if it wasn't stopped by any player.
        /// </summary>
        /// <param name="ball">The ball.</param>
        /// <param name="whichGoalLine">The number of the goal line.</param>
        /// <returns>Intersection with the goal line point. Null if there is not such intersection (ball 
        /// is not moving to the goal line or it is moving too slow).</returns>
        private Vector GetIntersectionWithGoalLine(Ball ball, int whichGoalLine)
        {
            double ballMovementToIntersectionVectorRatio;
            if (ball.Movement.X == 0)
                return null;

            if (whichGoalLine == 1)
                ballMovementToIntersectionVectorRatio = -ball.Position.X / ball.Movement.X;
            else
                ballMovementToIntersectionVectorRatio = (110 - ball.Position.X) / ball.Movement.X;

            if (ballMovementToIntersectionVectorRatio < 0)
                return null;

            var intersection = new Vector
            {
                X = ball.Position.X + ballMovementToIntersectionVectorRatio * ball.Movement.X,
                Y = ball.Position.Y + ballMovementToIntersectionVectorRatio * ball.Movement.Y
            };

            // calculate speed at intersection
            var distanceFromIntersection =
                Math.Sqrt(Math.Pow(intersection.X - ball.Position.X, 2) - Math.Pow(intersection.Y - ball.Position.Y, 2));

            // at^2 + 2(v_0)t -s = 0, where v_0 = start ball speed, a = acceleration, s = distance, t = time
            // from that equation we calculate t
            var discriminant = 4 * Math.Pow(ball.CurrentSpeed, 2) - 8 * BallDecerelation * distanceFromIntersection;
            if (discriminant < 0)
                return null; // ball will stop -> no intersection reached

            var time = (ball.CurrentSpeed - Math.Sqrt(discriminant)) / BallDecerelation;

            // final speed = v + a * t
            var speedAtIntersection = ball.CurrentSpeed + BallDecerelation * time;

            // is speed higher than minimal shot speed (4 for now)
            if (speedAtIntersection <= 4)
                return null;

            return intersection;
        }

        /// <summary>
        /// Gets the kick winner in accordance to the kickers near the ball parameters.
        /// </summary>
        /// <param name="kickers">The kickers near the ball.</param>
        /// <returns>The kick winner.</returns>
        private FootballPlayer GetKickWinner(FootballPlayer[] kickers)
        {
            if (kickers.Length == 0)
                return null;
            if (LastKicker != null && kickers.Contains(LastKicker))
            {
                kickers = kickers.Where(k => k != LastKicker).ToArray();
                var lastKickerWeight = 1.5 + LastKicker.Possession;
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
