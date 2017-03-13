using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.CustomDataTypes;
using FootballAIGame.MatchSimulation.Messages;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.MatchSimulation
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
        public const int PlayerTimeForOneStep = 1000000; // [ms]

        /// <summary>
        /// The time in milliseconds of one simulation step.
        /// </summary>
        public const int StepInterval = 200; // [ms]

        /// <summary>
        /// The maximum allowed acceleration in meters per second squared of football player.
        /// </summary>
        public const double MaxAcceleration = 5; // [m/s/s]

        /// <summary>
        /// The ball deceleration in meters per second squared.
        /// </summary>
        public const double BallDecerelation = 1.5; // [m/s/s]

        public const double BallMaxDistanceForKick = 2; // [m]

        /// <summary>
        /// The minimal opponent length from kickoff in meters.
        /// </summary>
        public const double MinimalOpponentDirectionFromKickoff = 6; // [m]

        /// <summary>
        /// The maximum number of errors of the same kind in the error log.
        /// </summary>
        public const int MaximumNumberOfSameKindErrorInLog = 5;

        /// <summary>
        /// Specifies how big the speed/acceleration correction needs to be
        /// for it to be reported (because of rounding errors).
        /// e.g. If max speed is 10 then the speed needs to be greater than 10 * AllowableRange
        /// for it to be saved to the error log.
        /// </summary>
        public const double MinReportableCorrection = 1.01;

        /// <summary>
        /// The football field height in meters.
        /// </summary>
        public const double FieldHeight = 75; // [m]

        /// <summary>
        /// The football field width in meters.
        /// </summary>
        public const double FieldWidth = 110; // [m]

        #endregion

        /// <summary>
        /// Gets the current simulation task.
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
        public IClientCommunicator Player1AiConnection { get; private set; }

        /// <summary>
        /// Gets the second player AI <see cref="ClientConnection"/>.
        /// </summary>
        /// <value>
        /// The player2 AI connection.
        /// </value>
        public IClientCommunicator Player2AiConnection { get; private set; }

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
        //public string Winner { get; set; }

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

        //public string Player1ErrorLog { get; set; }

        //public string Player2ErrorLog { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Random"/> used for generating random numbers.
        /// </summary>
        public static Random Random { get; set; } = new Random();

        public MatchInfo MatchInfo { get; set; }

        private GameState GameState { get; set; }
        private int Ping1 { get; set; }
        private int Ping2 { get; set; }
        private Task<ClientMessage> Message1 { get; set; }
        private Task<ClientMessage> Message2 { get; set; }
        private FootballPlayer LastKicker { get; set; }
        private Team WhoIsOnLeft { get; set; }

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
        //private int CurrentScore1 { get; set; }
        //private int CurrentScore2 { get; set; }
        private Dictionary<SimulationError.ErrorType, int> NumberOfPlayer1Errors { get; set; }
        private Dictionary<SimulationError.ErrorType, int> NumberOfPlayer2Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchSimulator"/> class.
        /// </summary>
        /// <param name="player1AiConnection">The player1 AI connection that will play the match.</param>
        /// <param name="player2AiConnection">The player2 AI connection that will play the match.</param>
        public MatchSimulator(IClientCommunicator player1AiConnection, IClientCommunicator player2AiConnection)
        {
            this.Player1AiConnection = player1AiConnection;
            this.Player2AiConnection = player2AiConnection;

            NumberOfPlayer1Errors = new Dictionary<SimulationError.ErrorType, int>();
            NumberOfPlayer2Errors = new Dictionary<SimulationError.ErrorType, int>();

            foreach (SimulationError.ErrorType value in Enum.GetValues(typeof(SimulationError.ErrorType)))
            {
                NumberOfPlayer1Errors.Add(value, 0);
                NumberOfPlayer2Errors.Add(value, 0);
            }
        }

        /// <summary>
        /// Simulates the match.
        /// </summary>
        /// <returns>Task that will be completed with simulation.</returns>
        public async Task SimulateMatchAsync()
        {
            CurrentSimulationTask = SimulateAsync();
            await CurrentSimulationTask;
        }

        /// <summary>
        /// Simulates the match. <see cref="SimulateMatchAsync"/> is it's public wrapper, that is used to set 
        /// <see cref="CurrentSimulationTask"/> property.
        /// </summary>
        private async Task SimulateAsync()
        {
            await ProcessSimulationStartAsync();

            var firstBall = Random.Next(1, 3); // max is excluded

            try
            {
                // first half
                WhoIsOnLeft = Team.FirstPlayer;
                SetStartingPositions(firstBall == 1 ? Team.FirstPlayer : Team.SecondPlayer);
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
                    await ProcessSimulationStepAsync(step);
                    if (step % 100 == 0)
                        Console.WriteLine(step);
                }

                // second half
                WhoIsOnLeft = Team.SecondPlayer;
                SetStartingPositions(firstBall == 1 ? Team.SecondPlayer : Team.FirstPlayer);
                for (var step = NumberOfSimulationSteps / 2; step < NumberOfSimulationSteps; step++)
                {
                    if (Player1CancelRequested || Player2CancelRequested || !Player1AiConnection.IsActive ||
                        !Player2AiConnection.IsActive)
                    {
                        ProcessSimulationEnd();
                        return;
                    }

                    CurrentStep = step;
                    await ProcessSimulationStepAsync(step);
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
        private async Task ProcessSimulationStartAsync()
        {
            Console.WriteLine($"Starting simulation between {Player1AiConnection.PlayerName} " +
                              $"and {Player2AiConnection.PlayerName}");

            // 1. check pings
            //Ping1 = Player1AiConnection.PingTimeAverage();
            //Ping2 = Player2AiConnection.PingTimeAverage();
            //Console.WriteLine($"{Player1Connection.PlayerName} ping = {Ping1}\n" +
            //                  $"{Player2Connection.PlayerName} ping = {Ping2}");

            MatchInfo = new MatchInfo();

            GameState = new GameState
            {
                Ball = new Ball(),
                FootballPlayers = new FootballPlayer[22],
                KickOff = false
            };

            await ProcessGettingParametersAsync();

            Message1 = Player1AiConnection.ReceiveClientMessageAsync();
            Message2 = Player2AiConnection.ReceiveClientMessageAsync();
        }

        /// <summary>
        /// Processes the simulation step. Client action are retrieved and the game state is updated
        /// accordingly.
        /// </summary>
        /// <param name="step">The step number.</param>
        /// <returns></returns>
        private async Task ProcessSimulationStepAsync(int step)
        {
            ActionMessage actionMessage1 = null;
            ActionMessage actionMessage2 = null;
            GameState.Step = step;
            GameState.KickOff = GameState.KickOff || GameState.Step == NumberOfSimulationSteps / 2 ||
                GameState.Step == 0;

            //await Task.Delay(20); // wait TODO delete
            var receiveActionMessage1 = Player1AiConnection.ReceiveActionMessageAsync(step);
            var receiveActionMessage2 = Player2AiConnection.ReceiveActionMessageAsync(step);

            await Player1AiConnection.TrySendAsync("GET ACTION");
            await Player1AiConnection.TrySendAsync(GameState, 1);

            await Player2AiConnection.TrySendAsync("GET ACTION");
            await Player2AiConnection.TrySendAsync(GameState, 2);
            //Console.WriteLine($"{GameState.Step} sent!");

            GameState.KickOff = false; // reset for next step!

            var getMessage1Task = Task.WhenAny(receiveActionMessage1, Task.Delay(Ping1 + PlayerTimeForOneStep));
            var getMessage2Task = Task.WhenAny(receiveActionMessage2, Task.Delay(Ping2 + PlayerTimeForOneStep));

            var getMessage1Result = await getMessage1Task;
            var getMessage2Result = await getMessage2Task;
            //Console.WriteLine($"{GameState.Step} received actions.");

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

            UpdateMatch(actionMessage1, actionMessage2);
            SaveState();
        }

        /// <summary>
        /// Processes the simulation end.
        /// </summary>
        private void ProcessSimulationEnd()
        {
            //Console.WriteLine($"Ending simulation between {Player1AiConnection.PlayerName}:{Player1AiConnection.AiName} " +
            //                 $"and {Player2AiConnection.PlayerName}:{Player2AiConnection.AiName}");

            // default winner
            MatchInfo.Winner = MatchInfo.Team1Statistics.Goals > MatchInfo.Team2Statistics.Goals
                ? Team.FirstPlayer
                : MatchInfo.Team2Statistics.Goals > MatchInfo.Team1Statistics.Goals ? Team.SecondPlayer : (Team?)null;

            if (Message1.IsFaulted || !Player1AiConnection.IsActive)
            {
                MatchInfo.Winner = Team.SecondPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.FirstPlayer,
                    Time = CurrentTime,
                    Type = SimulationError.ErrorType.Disconnection
                });
            }
            if (Message2.IsFaulted || !Player2AiConnection.IsActive)
            {
                MatchInfo.Winner = Team.FirstPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.SecondPlayer,
                    Time = CurrentTime,
                    Type = SimulationError.ErrorType.Disconnection
                });
            }

            if (Player1CancelRequested)
            {
                MatchInfo.Winner = Team.SecondPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.FirstPlayer,
                    Time = CurrentTime,
                    Type = SimulationError.ErrorType.Cancel
                });
            }
            if (Player2CancelRequested)
            {
                MatchInfo.Winner = Team.FirstPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.SecondPlayer,
                    Time = CurrentTime,
                    Type = SimulationError.ErrorType.Disconnection
                });
            }

        }

        /// <summary>
        /// Processes getting of the players parameters from both AIs.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessGettingParametersAsync()
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
                        par.Speed + par.KickPower + par.Possession + par.Precision - 1 <= 0.01) // todo improve
                    {
                        GameState.FootballPlayers[i] = getParameters1.Result[i];
                    }
                    else // default parameters
                    {
                        GameState.FootballPlayers[i] = new FootballPlayer(i)
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
                    GameState.FootballPlayers[i] = new FootballPlayer(i)
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
                        GameState.FootballPlayers[i + 11].Id = i + 11; // parse message will assigns i !
                    }
                    else // default parameters
                    {
                        GameState.FootballPlayers[i + 11] = new FootballPlayer(i + 11)
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
                    GameState.FootballPlayers[i + 11] = new FootballPlayer(i + 11)
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
        private void SetStartingPositions(Team whoHasBall)
        {
            var players = GameState.FootballPlayers;

            /* TEAM 1 */
            // goalie
            players[0].Position.X = 11;
            players[0].Position.Y = 75f / 2;

            // defenders positions
            players[1].Position.X = 16.5;
            players[1].Position.Y = 15;
            players[2].Position.X = 16.5;
            players[2].Position.Y = 75 / 2f - 12;
            players[3].Position.X = 16.5;
            players[3].Position.Y = 75 / 2f + 12;
            players[4].Position.X = 16.5;
            players[4].Position.Y = 60;

            // midfielders
            players[5].Position.X = 33;
            players[5].Position.Y = 75 / 2f - 12;
            players[6].Position.X = 33;
            players[6].Position.Y = 75 / 2f;
            players[7].Position.X = 33;
            players[7].Position.Y = 75 / 2f + 12;
            players[8].Position.X = 33;
            players[8].Position.Y = 60;

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
            if ((whoHasBall == Team.FirstPlayer && WhoIsOnLeft == Team.FirstPlayer) ||
                (whoHasBall == Team.SecondPlayer && WhoIsOnLeft == Team.SecondPlayer))
            {
                players[10].Position.X = 54;
                players[10].Position.Y = 75 / 2f;
            }
            else
            {
                players[21].Position.X = 56;
                players[21].Position.Y = 75 / 2f;
            }

            if (WhoIsOnLeft == Team.SecondPlayer)
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
            UpdateBall(player1Action, player2Action);
            UpdatePlayers(player1Action, player2Action);
            HandleOuts();
            HandleGoals();
        }

        /// <summary>
        /// Saves the current <see cref="GameState"/> to the <see cref="MatchInfo"/>.
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
                MatchInfo.MatchData.Add(num);
        }

        /// <summary>
        /// Gets the player parameters from the given AI.
        /// </summary>
        /// <param name="playerConnection">The player AI connection.</param>
        /// <returns>The array of football players with their parameters set accordingly.</returns>
        private static async Task<FootballPlayer[]> GetPlayerParameters(IClientCommunicator playerConnection)
        {
            ClientMessage message;

            while (true)
            {
                await playerConnection.TrySendAsync("GET PARAMETERS");
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

                    if (double.IsNaN(action.Movement.X) || double.IsNaN(action.Movement.Y) ||
                        double.IsInfinity(action.Movement.X) || double.IsInfinity(action.Movement.Y))
                    {
                        action.Movement = new Vector(0, 0);
                        if (NumberOfPlayer1Errors[SimulationError.ErrorType.InvalidMovementVector]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.FirstPlayer,
                                AffectedPlayerNumber = i,
                                Type = SimulationError.ErrorType.InvalidMovementVector
                            });
                    }

                    var acceleration = new Vector(action.Movement.X - player.Movement.X,
                        action.Movement.Y - player.Movement.Y);

                    var accelerationValue = acceleration.Length * Math.Pow(1000.0 / StepInterval, 2); // [m/s/s]

                    if (accelerationValue > MaxAcceleration)
                    {
                        var q = MaxAcceleration / accelerationValue;
                        var fixedAcceleration = new Vector(acceleration.X * q, acceleration.Y * q);

                        action.Movement.X = player.Movement.X + fixedAcceleration.X;
                        action.Movement.Y = player.Movement.Y + fixedAcceleration.Y;

                        if (accelerationValue > MaxAcceleration * MinReportableCorrection &&
                            NumberOfPlayer1Errors[SimulationError.ErrorType.TooHighAcceleration]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.FirstPlayer,
                                AffectedPlayerNumber = i,
                                Type = SimulationError.ErrorType.TooHighAcceleration
                            });

                    }


                    player.Movement.X = action.Movement.X;
                    player.Movement.Y = action.Movement.Y;
                    var newSpeed = player.CurrentSpeed;

                    if (newSpeed > player.MaxSpeed)
                    {
                        // too high speed
                        player.Movement.X *= player.MaxSpeed / newSpeed;
                        player.Movement.Y *= player.MaxSpeed / newSpeed;

                        if (newSpeed > player.MaxSpeed * MinReportableCorrection &&
                            NumberOfPlayer1Errors[SimulationError.ErrorType.TooHighSpeed]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.FirstPlayer,
                                AffectedPlayerNumber = i,
                                Type = SimulationError.ErrorType.TooHighSpeed
                            });
                    }

                    // stop player from going too far from field
                    StopPlayerFromGoingOutside(player);

                    // apply
                    player.Position.X += player.Movement.X;
                    player.Position.Y += player.Movement.Y;
                }
            }
            else
            {
                // default (nothing for now)
                // todo add error message
                Console.WriteLine(Player1AiConnection.PlayerName + " timeout.");
            }

            if (player2Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i + 11];
                    var action = player2Action.PlayersActions[i];

                    if (double.IsNaN(action.Movement.X) || double.IsNaN(action.Movement.Y) ||
                      double.IsInfinity(action.Movement.X) || double.IsInfinity(action.Movement.Y))
                    {
                        action.Movement = new Vector(0, 0);
                        if (NumberOfPlayer2Errors[SimulationError.ErrorType.InvalidMovementVector]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.SecondPlayer,
                                AffectedPlayerNumber = i,
                                Type = SimulationError.ErrorType.InvalidMovementVector
                            });
                    }

                    var acceleration = new Vector(action.Movement.X - player.Movement.X,
                        action.Movement.Y - player.Movement.Y);

                    var accelerationValue = acceleration.Length * Math.Pow(1000.0 / StepInterval, 2); // [m/s]

                    if (accelerationValue > MaxAcceleration)
                    {
                        var q = MaxAcceleration / accelerationValue;
                        var fixedAcceleration = new Vector(acceleration.X * q, acceleration.Y * q);

                        action.Movement.X = (float)(player.Movement.X + fixedAcceleration.X);
                        action.Movement.Y = (float)(player.Movement.Y + fixedAcceleration.Y);

                        if (accelerationValue > MaxAcceleration * MinReportableCorrection &&
                            NumberOfPlayer2Errors[SimulationError.ErrorType.TooHighAcceleration]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.SecondPlayer,
                                AffectedPlayerNumber = i,
                                Type = SimulationError.ErrorType.TooHighAcceleration
                            });
                    }


                    player.Movement.X = action.Movement.X;
                    player.Movement.Y = action.Movement.Y;
                    var newSpeed = player.CurrentSpeed;

                    if (newSpeed > player.MaxSpeed)
                    {
                        player.Movement.X *= (float)(player.MaxSpeed / newSpeed);
                        player.Movement.Y *= (float)(player.MaxSpeed / newSpeed);

                        if (newSpeed > player.MaxSpeed * MinReportableCorrection &&
                            NumberOfPlayer2Errors[SimulationError.ErrorType.TooHighSpeed]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.SecondPlayer,
                                AffectedPlayerNumber = i,
                                Type = SimulationError.ErrorType.TooHighSpeed
                            });
                    }

                    // stop player from going too far from field
                    StopPlayerFromGoingOutside(player);

                    // apply
                    player.Position.X += player.Movement.X;
                    player.Position.Y += player.Movement.Y;

                }
            }
            else
            {
                // default (nothing for now)
                // todo add error message
                Console.WriteLine(Player2AiConnection.PlayerName + " timeout.");
            }
        }

        private void StopPlayerFromGoingOutside(FootballPlayer player)
        {
            var y0 = player.Position.Y;
            var y1 = player.Position.Y + player.Movement.Y;
            var x0 = player.Position.X;
            var x1 = player.Position.X + player.Movement.X;

            var k = 1.0;

            if (y1 < -5)
                k = (-5 - y0) / y1;
            if (y1 > FieldHeight + 5)
                k = (FieldHeight + 5 - y0) / y1;
            if (x1 < -5)
                k = (-5 - x0) / x1;
            if (x1 > FieldWidth + 5)
                k = (FieldWidth + 5 - x0) / x1;

            player.Movement = new Vector(player.Movement.X * k, player.Movement.Y * k);
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

                var kickX = GameState.FootballPlayers[i].Kick.X;
                var kickY = GameState.FootballPlayers[i].Kick.Y;

                if (double.IsNaN(kickX) || double.IsNaN(kickY) ||
                      double.IsInfinity(kickX) || double.IsInfinity(kickY))
                {
                    GameState.FootballPlayers[i].Kick = new Vector(0, 0);
                    if (NumberOfPlayer1Errors[SimulationError.ErrorType.InvalidKickVector]++ < MaximumNumberOfSameKindErrorInLog)
                        MatchInfo.Errors.Add(new SimulationError()
                        {
                            Team = Team.FirstPlayer,
                            Time = CurrentTime,
                            AffectedPlayerNumber = i,
                            Type = SimulationError.ErrorType.InvalidKickVector
                        });
                }

            }
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i + 11].Kick.X = player2Action?.PlayersActions[i].Kick.X ?? 0;
                GameState.FootballPlayers[i + 11].Kick.Y = player2Action?.PlayersActions[i].Kick.Y ?? 0;


                var kickX = GameState.FootballPlayers[i + 11].Kick.X;
                var kickY = GameState.FootballPlayers[i + 11].Kick.Y;

                if (double.IsNaN(kickX) || double.IsNaN(kickY) ||
                      double.IsInfinity(kickX) || double.IsInfinity(kickY))
                {
                    GameState.FootballPlayers[i + 11].Kick = new Vector(0, 0);
                    if (NumberOfPlayer2Errors[SimulationError.ErrorType.InvalidKickVector]++ < MaximumNumberOfSameKindErrorInLog)
                        MatchInfo.Errors.Add(new SimulationError()
                        {
                            Team = Team.SecondPlayer,
                            Time = CurrentTime,
                            AffectedPlayerNumber = i,
                            Type = SimulationError.ErrorType.InvalidKickVector
                        });
                }
            }

            var playersNearBallKicking = GameState.FootballPlayers.Where(
                p => Vector.DistanceBetween(GameState.Ball.Position, p.Position) <= BallMaxDistanceForKick &&
                (p.Kick.X != 0 || p.Kick.Y != 0));

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
                var angleDevation = 1 / 4.0 * (0.4 - kickWinner.Precision) * (Random.NextDouble() * 2 - 1);

                // rotation applied (deviation)
                GameState.Ball.Movement.X = (float)(Math.Cos(angleDevation) * GameState.Ball.Movement.X -
                                                Math.Sin(angleDevation) * GameState.Ball.Movement.Y);
                GameState.Ball.Movement.Y = (float)(Math.Sin(angleDevation) * GameState.Ball.Movement.X +
                                                Math.Cos(angleDevation) * GameState.Ball.Movement.Y);

                var newSpeed = GameState.Ball.CurrentSpeed;
                var maxAllowedSpeed = LastKicker.MaxKickSpeed;
                if (newSpeed > maxAllowedSpeed)
                {
                    GameState.Ball.Movement.X *= (float)(maxAllowedSpeed / newSpeed);
                    GameState.Ball.Movement.Y *= (float)(maxAllowedSpeed / newSpeed);

                    if (kickWinner.Id < 11)
                    {
                        if (newSpeed > maxAllowedSpeed * MinReportableCorrection &&
                            NumberOfPlayer1Errors[SimulationError.ErrorType.TooStrongKick]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Team = Team.FirstPlayer,
                                Time = CurrentTime,
                                AffectedPlayerNumber = kickWinner.Id,
                                Type = SimulationError.ErrorType.TooStrongKick
                            });
                    }
                    else
                    {
                        if (newSpeed > maxAllowedSpeed * MinReportableCorrection &&
                            NumberOfPlayer2Errors[SimulationError.ErrorType.TooStrongKick]++ < MaximumNumberOfSameKindErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Team = Team.SecondPlayer,
                                Time = CurrentTime,
                                AffectedPlayerNumber = kickWinner.Id - 11,
                                Type = SimulationError.ErrorType.TooStrongKick
                            });
                    }
                }
            }

            // update ball position
            GameState.Ball.Position.X += GameState.Ball.Movement.X;
            GameState.Ball.Position.Y += GameState.Ball.Movement.Y;

            // ball deceleration (for the next step)
            var ratio = (GameState.Ball.CurrentSpeed - (BallDecerelation * StepInterval / 1000)) /
                GameState.Ball.CurrentSpeed;
            if (ratio < 0)
                ratio = 0;
            GameState.Ball.Movement.X *= (float)ratio;
            GameState.Ball.Movement.Y *= (float)ratio;
        }

        /// <summary>
        /// Handles out of play rules simulation.
        /// </summary>
        private void HandleOuts()
        {
            var lastTeam = Team.FirstPlayer; // default (will be set correctly)
            var players = GameState.FootballPlayers;
            var ball = GameState.Ball;

            for (var i = 0; i < 22; i++)
                if (LastKicker == GameState.FootballPlayers[i])
                    lastTeam = i < 11 ? Team.FirstPlayer : Team.SecondPlayer;

            // corner kicks or goal kicks (goal line crossed)
            if (ball.Position.Y > 75f / 2 + 7.32 / 2 || ball.Position.Y < 75f / 2 - 7.32 / 2) // not goals
            {
                if (GameState.Ball.Position.X > 110)
                {
                    GameState.KickOff = true;

                    if (WhoIsOnLeft == lastTeam)
                    {
                        var goalKeeper = lastTeam == Team.FirstPlayer ? players[11] : players[0];
                        // goal kick
                        goalKeeper.Position.X = 110 - 16.5f;
                        goalKeeper.Position.Y = 75 / 2f;
                        goalKeeper.Movement.X = 0;
                        goalKeeper.Movement.Y = 0;

                        ball.Position.X = goalKeeper.Position.X;
                        ball.Position.Y = goalKeeper.Position.Y;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        if (lastTeam == Team.FirstPlayer)
                            MatchInfo.Team1Statistics.Shots++;
                        else
                            MatchInfo.Team2Statistics.Shots++;
                    }
                    else
                    {
                        // corner kick
                        ball.Position.X = 109;
                        ball.Position.Y = ball.Position.Y >= 75f / 2 + 7.32 / 2 ? 74 : 1;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        var nearestPlayerFromOppositeTeam =
                            GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? 2 : 1);

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
                    GameState.KickOff = true;

                    if (WhoIsOnLeft != lastTeam)
                    {
                        var goalKeeper = lastTeam == Team.FirstPlayer ? players[11] : players[0];

                        // goal kick
                        goalKeeper.Position.X = 16.5f;
                        goalKeeper.Position.Y = 75 / 2f;
                        goalKeeper.Movement.X = 0;
                        goalKeeper.Movement.Y = 0;

                        ball.Position.X = goalKeeper.Position.X;
                        ball.Position.Y = goalKeeper.Position.Y;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        if (lastTeam == Team.FirstPlayer)
                            MatchInfo.Team1Statistics.Shots++;
                        else
                            MatchInfo.Team2Statistics.Shots++;
                    }
                    else
                    {
                        // corner kick
                        ball.Position.X = 1;
                        ball.Position.Y = ball.Position.Y >= 75f / 2 + 7.32 / 2 ? 74 : 1;
                        ball.Movement.X = 0f;
                        ball.Movement.Y = 0f;

                        var nearestPlayerFromOppositeTeam =
                           GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? 2 : 1);

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
                GameState.KickOff = true;

                ball.Position.Y = 1;
                ball.Movement.X = 0f;
                ball.Movement.Y = 0f;

                var nearestPlayerFromOppositeTeam =
                          GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? 2 : 1);

                nearestPlayerFromOppositeTeam.Position.X = ball.Position.X;
                nearestPlayerFromOppositeTeam.Position.Y = 0;
                nearestPlayerFromOppositeTeam.Movement.X = 0;
                nearestPlayerFromOppositeTeam.Movement.Y = 0;

                // push all opponent players aways from the kickoff position
                PushPlayersFromPosition(lastTeam, ball.Position);

            }
            if (ball.Position.Y > 75)
            {
                GameState.KickOff = true;

                ball.Position.Y = 74;
                ball.Movement.X = 0f;
                ball.Movement.Y = 0f;

                var nearestPlayerFromOppositeTeam =
                    GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? 2 : 1);

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
            var lastTeam = Team.FirstPlayer; // default (will be set correctly)
            var ball = GameState.Ball;

            for (var i = 0; i < 22; i++)
                if (LastKicker == GameState.FootballPlayers[i])
                    lastTeam = i < 11 ? Team.FirstPlayer : Team.SecondPlayer;

            if (ball.Position.X < 0 && ball.Position.Y < 75f / 2 + 7.32 / 2 && ball.Position.Y > 75f / 2 - 7.32 / 2)
            {
                GameState.KickOff = true;

                var teamThatScored = WhoIsOnLeft == Team.FirstPlayer ? Team.SecondPlayer : Team.FirstPlayer;

                if (WhoIsOnLeft == Team.FirstPlayer)
                {
                    MatchInfo.Team2Statistics.Goals++;
                    if (lastTeam == Team.SecondPlayer)
                    {
                        MatchInfo.Team2Statistics.ShotsOnTarget++;
                        MatchInfo.Team2Statistics.Shots++;
                    }
                }
                if (WhoIsOnLeft == Team.SecondPlayer)
                {
                    MatchInfo.Team1Statistics.Goals++;
                    if (lastTeam == Team.FirstPlayer)
                    {
                        MatchInfo.Team1Statistics.ShotsOnTarget++;
                        MatchInfo.Team1Statistics.Shots++;
                    }
                }

                var scoringPlayerNumber = 0;
                for (var i = 0; i < 22; i++)
                    if (GameState.FootballPlayers[i] == LastKicker)
                        scoringPlayerNumber = i < 11 ? i : i - 11;

                MatchInfo.Goals.Add(new Goal()
                {
                    ScoreTime = CurrentTime,
                    TeamThatScored = teamThatScored,
                    ScorerNumber = scoringPlayerNumber
                });

                SetStartingPositions(WhoIsOnLeft);
            }
            else if (ball.Position.X > 110 && ball.Position.Y < 75f / 2 + 7.32 / 2 && ball.Position.Y > 75f / 2 - 7.32 / 2)
            {
                GameState.KickOff = true;

                var teamThatScored = WhoIsOnLeft == Team.SecondPlayer ? Team.SecondPlayer : Team.FirstPlayer;

                if (WhoIsOnLeft == Team.FirstPlayer)
                {
                    MatchInfo.Team1Statistics.Goals++;
                    if (lastTeam == Team.FirstPlayer)
                    {
                        MatchInfo.Team1Statistics.ShotsOnTarget++;
                        MatchInfo.Team1Statistics.Shots++;
                    }
                }
                if (WhoIsOnLeft == Team.SecondPlayer)
                {
                    MatchInfo.Team2Statistics.Goals++;
                    if (lastTeam == Team.SecondPlayer)
                    {
                        MatchInfo.Team2Statistics.ShotsOnTarget++;
                        MatchInfo.Team2Statistics.Shots++;
                    }
                }

                var scoringPlayerNumber = 0;
                for (var i = 0; i < 22; i++)
                    if (GameState.FootballPlayers[i] == LastKicker)
                        scoringPlayerNumber = i < 11 ? i : i - 11;

                MatchInfo.Goals.Add(new Goal()
                {
                    ScoreTime = CurrentTime,
                    TeamThatScored = teamThatScored,
                    ScorerNumber = scoringPlayerNumber
                });

                SetStartingPositions(WhoIsOnLeft == Team.FirstPlayer ? Team.SecondPlayer : Team.FirstPlayer);
            }
        }

        /// <summary>
        /// Handles the stopped shots. Updates shots and shots on target statistics
        /// accordingly.
        /// </summary>
        /// <param name="currentKickWinner">The current kick winner.</param>
        private void HandleStoppedShots(FootballPlayer currentKickWinner)
        {
            var currentWinnerTeam = Team.FirstPlayer; // default (will be set correctly)
            var lastWinnerTeam = Team.FirstPlayer; // default (will be set correctly)

            for (var i = 0; i < 11; i++)
            {
                if (GameState.FootballPlayers[i] == currentKickWinner)
                    currentWinnerTeam = i < 11 ? Team.FirstPlayer : Team.SecondPlayer;
                if (GameState.FootballPlayers[i] == LastKicker)
                    lastWinnerTeam = i < 11 ? Team.FirstPlayer : Team.SecondPlayer;
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
                if (lastWinnerTeam == Team.FirstPlayer)
                    MatchInfo.Team1Statistics.Shots++;
                else
                    MatchInfo.Team2Statistics.Shots++;
                if (wasBallGoingToGoalPost1)
                {
                    if (lastWinnerTeam == Team.FirstPlayer)
                        MatchInfo.Team1Statistics.ShotsOnTarget++;
                    else
                        MatchInfo.Team2Statistics.ShotsOnTarget++;
                }
            }

            if (wasBallGoingToGoalLine2 && lastWinnerTeam == WhoIsOnLeft)
            {
                if (lastWinnerTeam == Team.FirstPlayer)
                    MatchInfo.Team1Statistics.Shots++;
                else
                    MatchInfo.Team2Statistics.Shots++;

                if (wasBallGoingToGoalPost1)
                {
                    if (lastWinnerTeam == Team.FirstPlayer)
                        MatchInfo.Team1Statistics.ShotsOnTarget++;
                    else
                        MatchInfo.Team2Statistics.ShotsOnTarget++;
                }
            }


        }

        /// <summary>
        /// Pushes the players from the given team away from the given <see cref="position"/> in 
        /// accordance to the <see cref="MinimalOpponentDirectionFromKickoff"/>.
        /// Used for pushing opponent players from kickoffs.
        /// </summary>
        /// <param name="teamToBePushed">The team to be pushed.</param>
        /// <param name="position">The position from which the players will be pushed aways.</param>
        private void PushPlayersFromPosition(Team teamToBePushed, Vector position)
        {
            var toBePushedPlayers = new List<FootballPlayer>();

            for (var i = 0; i < 11; i++)
            {
                var player = GameState.FootballPlayers[teamToBePushed == Team.FirstPlayer ? i : i + 11];
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
            if (Math.Abs(ball.Movement.X) < 0.0001) // doesn't move
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