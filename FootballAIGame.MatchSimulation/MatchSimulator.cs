using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FootballAIGame.MatchSimulation.CustomDataTypes;
using FootballAIGame.MatchSimulation.Messages;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.MatchSimulation
{
    /// <summary>
    /// Provides the functionality to simulate football matches.
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
        public const int PlayerTimeForOneStep = 600; // [ms]

        /// <summary>
        /// The time of one simulation step in milliseconds.
        /// </summary>
        public const int StepInterval = 200; // [ms]

        /// <summary>
        /// The maximum allowed acceleration of football players in meters per second squared.
        /// </summary>
        public const double MaxAcceleration = 5; // [m/s/s]

        /// <summary>
        /// The ball's deceleration in meters per second squared.
        /// </summary>
        public const double BallDeceleration = 1.5; // [m/s/s]

        /// <summary>
        /// The ball's maximum allowed distance for kicking the ball in meters.
        /// </summary>
        public const double BallMaxDistanceForKick = 2; // [m]

        /// <summary>
        /// The opponents' minimal distance from the kickoff's position during kickoff in meters.
        /// </summary>
        public const double MinimalOpponentDistanceFromKickoff = 6; // [m]

        /// <summary>
        /// The maximum player Manhattan distance from  the football field in meters.
        /// </summary>
        public const double MaximumPlayerManhattanDistanceFromField = 5; // [m]

        /// <summary>
        /// The maximum number of errors with the same <see cref="SimulationErrorReason"/> that
        /// is saved to the error log.
        /// </summary>
        public const int MaximumNumberOfSameReasonErrorInLog = 5;

        /// <summary>
        /// Specifies how big the speed/acceleration correction needs to be
        /// for it to be reported (because of rounding errors). <para/>
        /// 
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
        /// Gets or sets a value indicating whether the information about the ongoing simulation
        /// should be written to the output.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is verbose; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerbose { get; set; }

        /// <summary>
        /// Gets the current simulation task.
        /// </summary>
        /// <value>
        /// The current simulation task.
        /// </value>
        public Task CurrentSimulationTask { get; private set; }

        /// <summary>
        /// Gets the first AI's <see cref="IClientCommunicator"/>.
        /// </summary>
        /// <value>
        /// The first AI's <see cref="IClientCommunicator"/>.
        /// </value>
        public IClientCommunicator AI1Communicator { get; private set; }

        /// <summary>
        /// Gets the second AI's <see cref="IClientCommunicator"/>.
        /// </summary>
        /// <value>
        /// The second AI's <see cref="IClientCommunicator"/>.
        /// </value>
        public IClientCommunicator AI2Communicator { get; private set; }

        /// <summary>
        /// Gets or sets the current simulation step.
        /// </summary>
        /// <value>
        /// The current simulation step.
        /// </value>
        public int CurrentStep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the first client has requested to cancel the match.
        /// </summary>
        /// <value>
        /// <c>true</c> if the first client has requested to cancel the match; otherwise, <c>false</c>.
        /// </value>
        public bool AI1CancelRequested { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the second client has requested to cancel the match.
        /// </summary>
        /// <value>
        /// <c>true</c> if second client has requested to cancel the match; otherwise, <c>false</c>.
        /// </value>
        public bool AI2CancelRequested { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Random" /> used for generating random numbers.
        /// </summary>
        /// <value>
        /// The <see cref="Random"/> instance.
        /// </value>
        public Random Random { get; set; } = new Random();

        /// <summary>
        /// Gets or sets the match information containing all the information about the simulated match.
        /// </summary>
        /// <value>
        /// The <see cref="MatchInfo"/> instance containing all the information about the simulated match.
        /// </value>
        public MatchInfo MatchInfo { get; set; }

        /// <summary>
        /// Gets or sets the current state of the game.
        /// </summary>
        /// <value>
        /// The state of the game.
        /// </value>
        private GameState GameState { get; set; }

        /// <summary>
        /// Gets or sets a task that represents the asynchronous receive operation from the first AI. 
        /// </summary>
        /// <value>
        /// The task that represents the asynchronous receive operation from the first AI. 
        /// </value>
        private Task<IClientMessage> AI1ReceiveMessage { get; set; }

        /// <summary>
        /// Gets or sets a task that represents the asynchronous receive operation from the second AI. 
        /// </summary>
        /// <value>
        /// The task that represents the asynchronous receive operation from the second AI. 
        /// </value>
        private Task<IClientMessage> AI2ReceiveMessage { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FootballPlayer"/> that was the last to kick the ball.
        /// </summary>
        /// <value>
        /// The last kicker.
        /// </value>
        private FootballPlayer LastKicker { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Team"/> that is on the left side of the football field.
        /// </summary>
        /// <value>
        /// The <see cref="Team"/> that is on the left side of the football field.
        /// </value>
        private Team WhoIsOnLeft { get; set; }

        /// <summary>
        /// Gets the current match's time. 
        /// </summary>
        /// <value>
        /// The current time in the following format: "minutes:seconds".
        /// </value>
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

        /// <summary>
        /// Gets or sets the <see cref="Dictionary{TKey,TValue}"/> containing for each <see cref="SimulationErrorReason"/>
        /// the number of first AI's errors with that reason that have happened until now.
        /// </summary>
        /// <value>
        /// The <see cref="Dictionary{TKey,TValue}"/> containing for each <see cref="SimulationErrorReason"/>
        /// the number of first AI's errors with that reason that have happened until now.
        /// </value>
        private Dictionary<SimulationErrorReason, int> NumberOfPlayer1Errors { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Dictionary{TKey,TValue}"/> containing for each <see cref="SimulationErrorReason"/>
        /// the number of second AI's errors with that reason that have happened until now.
        /// </summary>
        /// <value>
        /// The <see cref="Dictionary{TKey,TValue}"/> containing for each <see cref="SimulationErrorReason"/>
        /// the number of second AI's errors with that reason that have happened until now.
        /// </value>
        private Dictionary<SimulationErrorReason, int> NumberOfPlayer2Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchSimulator"/> class that is used to simulate the
        /// match between the specified AI's.
        /// </summary>
        /// <param name="firstAICommunicator">The first AI communicator.</param>
        /// <param name="secondAICommunicator">The second AI communicator.</param>
        public MatchSimulator(IClientCommunicator firstAICommunicator, IClientCommunicator secondAICommunicator)
        {
            AI1Communicator = firstAICommunicator;
            AI2Communicator = secondAICommunicator;

            NumberOfPlayer1Errors = new Dictionary<SimulationErrorReason, int>();
            NumberOfPlayer2Errors = new Dictionary<SimulationErrorReason, int>();

            foreach (SimulationErrorReason value in Enum.GetValues(typeof(SimulationErrorReason)))
            {
                NumberOfPlayer1Errors.Add(value, 0);
                NumberOfPlayer2Errors.Add(value, 0);
            }
        }

        /// <summary>
        /// Simulates the match asynchronously.
        /// </summary>
        /// <returns>The task that represents the asynchronous simulate operation.</returns>
        public async Task SimulateMatchAsync()
        {
            CurrentSimulationTask = SimulateAsync();
            await CurrentSimulationTask;
        }

        /// <summary>
        /// Simulates the match. <see cref="SimulateMatchAsync" /> is it's public wrapper, that is used to set
        /// <see cref="CurrentSimulationTask" />.
        /// </summary>
        /// <returns>The task that represents the asynchronous simulate operation.</returns>
        private async Task SimulateAsync()
        {
            await ProcessSimulationStartAsync();

            var firstBall = Random.Next(1, 3); // max is excluded

            try
            {
                // first half
                WhoIsOnLeft = Team.FirstPlayer;
                SetPlayersToStartingPositions(firstBall == 1 ? Team.FirstPlayer : Team.SecondPlayer);
                SaveState(); // save starting state

                for (var step = 0; step < NumberOfSimulationSteps / 2; step++)
                {
                    if (AI1CancelRequested || AI2CancelRequested || !AI1Communicator.IsLoggedIn ||
                        !AI2Communicator.IsLoggedIn)
                    {
                        ProcessSimulationEnd(step);
                        return;
                    }

                    CurrentStep = step;
                    await ProcessSimulationStepAsync(step);
                    //if (step % 100 == 0)
                    //    Console.WriteLine(step);
                }

                // second half
                WhoIsOnLeft = Team.SecondPlayer;
                SetPlayersToStartingPositions(firstBall == 1 ? Team.SecondPlayer : Team.FirstPlayer);
                for (var step = NumberOfSimulationSteps / 2; step < NumberOfSimulationSteps; step++)
                {
                    if (AI1CancelRequested || AI2CancelRequested || !AI1Communicator.IsLoggedIn ||
                        !AI2Communicator.IsLoggedIn)
                    {
                        ProcessSimulationEnd(step);
                        return;
                    }

                    CurrentStep = step;
                    await ProcessSimulationStepAsync(step);
                    //if (step % 100 == 0)
                    //    Console.WriteLine(step);
                }

                ProcessSimulationEnd(NumberOfSimulationSteps);
            }
            catch (Exception ex)
            {
                if (IsVerbose)
                {
                    Console.WriteLine("simulation exception");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Processes the simulation start asynchronously. New match is initialized. Players parameters and ping are set.
        /// </summary>
        /// <returns>The task that represents the asynchronous process operation.</returns>
        private async Task ProcessSimulationStartAsync()
        {
            if (IsVerbose)
            {
                Console.WriteLine($"Starting simulation between {AI1Communicator.PlayerName} " +
                                  $"and {AI2Communicator.PlayerName}");
            }

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
                IsKickOff = false
            };

            await ProcessGettingParametersAsync();

            AI1ReceiveMessage = AI1Communicator.ReceiveClientMessageAsync();
            AI2ReceiveMessage = AI2Communicator.ReceiveClientMessageAsync();
        }

        /// <summary>
        /// Processes the simulation step asynchronously. Client action are retrieved and the game state is updated
        /// accordingly.
        /// </summary>
        /// <param name="step">The step number.</param>
        /// <returns>The task that represents the asynchronous process operation.</returns>
        private async Task ProcessSimulationStepAsync(int step)
        {
            ActionMessage actionMessage1 = null;
            ActionMessage actionMessage2 = null;
            GameState.Step = step;
            GameState.IsKickOff = GameState.IsKickOff || GameState.Step == NumberOfSimulationSteps / 2 ||
                GameState.Step == 0;

            var receiveActionMessage1 = AI1Communicator.ReceiveActionMessageAsync(step);
            var receiveActionMessage2 = AI2Communicator.ReceiveActionMessageAsync(step);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await AI1Communicator.TrySendAsync("GET ACTION");
            await AI1Communicator.TrySendAsync(GameState, Team.FirstPlayer);

            await AI2Communicator.TrySendAsync("GET ACTION");
            await AI2Communicator.TrySendAsync(GameState, Team.SecondPlayer);

            GameState.IsKickOff = false; // reset for next step!

            Action<Task, TeamStatistics> logLatency = (task, teamStatistics) =>
            {
                task.ContinueWith((t) =>
                {
                    int time = (int)stopwatch.ElapsedMilliseconds;
                    teamStatistics.AverageActionLatency += time;
                });
            };

            logLatency(receiveActionMessage1, MatchInfo.Team1Statistics);
            logLatency(receiveActionMessage2, MatchInfo.Team2Statistics);

            var getMessage1Task = Task.WhenAny(receiveActionMessage1, Task.Delay(PlayerTimeForOneStep));
            var getMessage2Task = Task.WhenAny(receiveActionMessage2, Task.Delay(PlayerTimeForOneStep));

            var getMessage1Result = await getMessage1Task;
            var getMessage2Result = await getMessage2Task;

            if (AI1ReceiveMessage.IsFaulted || !AI1Communicator.IsLoggedIn || AI2ReceiveMessage.IsFaulted ||
                !AI2Communicator.IsLoggedIn)
            {
                return;
            }

            if (getMessage1Result == receiveActionMessage1 && !receiveActionMessage1.IsFaulted)
            {
                actionMessage1 = AI1ReceiveMessage.Result as ActionMessage;

                if (step < NumberOfSimulationSteps - 1)
                {
                    AI1ReceiveMessage = AI1Communicator.ReceiveClientMessageAsync();
                }
            }

            if (getMessage2Result == receiveActionMessage2 && !receiveActionMessage2.IsFaulted)
            {
                actionMessage2 = AI2ReceiveMessage.Result as ActionMessage;

                if (step < NumberOfSimulationSteps - 1)
                {
                    AI2ReceiveMessage = AI2Communicator.ReceiveClientMessageAsync();
                }
            }

            UpdateMatch(actionMessage1, actionMessage2);
            SaveState();
        }

        /// <summary>
        /// Processes the simulation end. Sets <see cref="MatchInfo" /> result values accordingly.
        /// </summary>
        /// <param name="numberOfCompletedSteps">The number of completed steps.</param>
        private void ProcessSimulationEnd(int numberOfCompletedSteps)
        {
            //Console.WriteLine($"Ending simulation between {Player1AiConnection.PlayerName}:{Player1AiConnection.AiName} " +
            //                 $"and {Player2AiConnection.PlayerName}:{Player2AiConnection.AiName}");

            // average action latencies division
            MatchInfo.Team1Statistics.AverageActionLatency /= numberOfCompletedSteps;
            MatchInfo.Team2Statistics.AverageActionLatency /= numberOfCompletedSteps;

            // default winner
            MatchInfo.Winner = MatchInfo.Team1Statistics.Goals > MatchInfo.Team2Statistics.Goals
                ? Team.FirstPlayer
                : MatchInfo.Team2Statistics.Goals > MatchInfo.Team1Statistics.Goals ? Team.SecondPlayer : (Team?)null;

            if (AI1ReceiveMessage.IsFaulted || !AI1Communicator.IsLoggedIn)
            {
                MatchInfo.Winner = Team.SecondPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.FirstPlayer,
                    Time = CurrentTime,
                    Reason = SimulationErrorReason.Disconnection
                });
            }
            if (AI2ReceiveMessage.IsFaulted || !AI2Communicator.IsLoggedIn)
            {
                MatchInfo.Winner = Team.FirstPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.SecondPlayer,
                    Time = CurrentTime,
                    Reason = SimulationErrorReason.Disconnection
                });
            }

            if (AI1CancelRequested)
            {
                MatchInfo.Winner = Team.SecondPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.FirstPlayer,
                    Time = CurrentTime,
                    Reason = SimulationErrorReason.Cancellation
                });
            }
            if (AI2CancelRequested)
            {
                MatchInfo.Winner = Team.FirstPlayer;
                MatchInfo.Errors.Add(new SimulationError()
                {
                    Team = Team.SecondPlayer,
                    Time = CurrentTime,
                    Reason = SimulationErrorReason.Disconnection
                });
            }

        }

        /// <summary>
        /// Processes the getting of the players parameters from both AIs.
        /// </summary>
        /// <returns>The task that represents the asynchronous parameters retrieving operation.</returns>
        private async Task ProcessGettingParametersAsync()
        {
            var getParametersCancellationSource1 = new CancellationTokenSource();
            var getParametersCancellationSource2 = new CancellationTokenSource();

            var getParameters1 = GetPlayerParametersAsync(AI1Communicator, getParametersCancellationSource1.Token);
            var getParameters2 = GetPlayerParametersAsync(AI2Communicator, getParametersCancellationSource2.Token);

            var result1 = await Task.WhenAny(getParameters1, Task.Delay(1000));
            var result2 = await Task.WhenAny(getParameters2, Task.Delay(1000));

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
                getParametersCancellationSource1.Cancel(); // stop waiting for parameters

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
                getParametersCancellationSource2.Cancel(); // stop waiting for parameters

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
        /// Sets players to their starting positions.
        /// </summary>
        /// <param name="whoHasBall">The team that is in possession of the ball.</param>
        private void SetPlayersToStartingPositions(Team whoHasBall)
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
        /// Updates the match to the next simulation step in accordance with the specified actions. 
        /// </summary>
        /// <param name="ai1Action">The first AI's action.</param>
        /// <param name="ai2Action">The second AI's action.</param>
        private void UpdateMatch(ActionMessage ai1Action, ActionMessage ai2Action)
        {
            UpdateBall(ai1Action, ai2Action);
            UpdatePlayers(ai1Action, ai2Action);
            ProcessOut();
            ProcessGoal();
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
        /// Gets the player parameters from the specified <see cref="IClientCommunicator" /> asynchronously.
        /// </summary>
        /// <param name="clientCommunicator">The client communicator.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The task that represents the asynchronous get operation.
        /// The value of the task's result is an array of football players
        /// with their parameters set accordingly.
        /// </returns>
        private static async Task<FootballPlayer[]> GetPlayerParametersAsync(IClientCommunicator clientCommunicator, CancellationToken cancellationToken)
        {
            IClientMessage message;

            while (true)
            {
                await clientCommunicator.TrySendAsync("GET PARAMETERS");

                message = await clientCommunicator.ReceiveClientMessageAsync();

                if (message is ParametersMessage)
                    break;

                cancellationToken.ThrowIfCancellationRequested();
            }

            return ((ParametersMessage)message).Players;
        }

        /// <summary>
        /// Updates the players' movements in accordance with the specified actions.
        /// </summary>
        /// <param name="ai1Action">The first AI's action.</param>
        /// <param name="ai2Action">The second AI's action.</param>
        private void UpdatePlayers(ActionMessage ai1Action, ActionMessage ai2Action)
        {
            if (ai1Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i];
                    var action = ai1Action.PlayersActions[i];

                    if (double.IsNaN(action.Movement.X) || double.IsNaN(action.Movement.Y) ||
                        double.IsInfinity(action.Movement.X) || double.IsInfinity(action.Movement.Y))
                    {
                        action.Movement = new Vector(0, 0);
                        if (NumberOfPlayer1Errors[SimulationErrorReason.InvalidMovementVector]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.FirstPlayer,
                                AffectedPlayerNumber = i,
                                Reason = SimulationErrorReason.InvalidMovementVector
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
                            NumberOfPlayer1Errors[SimulationErrorReason.TooHighAcceleration]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.FirstPlayer,
                                AffectedPlayerNumber = i,
                                Reason = SimulationErrorReason.TooHighAcceleration
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
                            NumberOfPlayer1Errors[SimulationErrorReason.TooHighSpeed]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.FirstPlayer,
                                AffectedPlayerNumber = i,
                                Reason = SimulationErrorReason.TooHighSpeed
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
                Console.WriteLine(AI1Communicator.PlayerName + " timeout.");
            }

            if (ai2Action != null)
            {
                // movement
                for (var i = 0; i < 11; i++)
                {
                    var player = GameState.FootballPlayers[i + 11];
                    var action = ai2Action.PlayersActions[i];

                    if (double.IsNaN(action.Movement.X) || double.IsNaN(action.Movement.Y) ||
                      double.IsInfinity(action.Movement.X) || double.IsInfinity(action.Movement.Y))
                    {
                        action.Movement = new Vector(0, 0);
                        if (NumberOfPlayer2Errors[SimulationErrorReason.InvalidMovementVector]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.SecondPlayer,
                                AffectedPlayerNumber = i,
                                Reason = SimulationErrorReason.InvalidMovementVector
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
                            NumberOfPlayer2Errors[SimulationErrorReason.TooHighAcceleration]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.SecondPlayer,
                                AffectedPlayerNumber = i,
                                Reason = SimulationErrorReason.TooHighAcceleration
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
                            NumberOfPlayer2Errors[SimulationErrorReason.TooHighSpeed]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Time = CurrentTime,
                                Team = Team.SecondPlayer,
                                AffectedPlayerNumber = i,
                                Reason = SimulationErrorReason.TooHighSpeed
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
                Console.WriteLine(AI2Communicator.PlayerName + " timeout.");
            }
        }

        /// <summary>
        /// Stops the player from going too far outside of the playing field.
        /// </summary>
        /// <param name="player">The player.</param>
        private static void StopPlayerFromGoingOutside(FootballPlayer player)
        {
            var y0 = player.Position.Y;
            var y1 = player.Position.Y + player.Movement.Y;
            var x0 = player.Position.X;
            var x1 = player.Position.X + player.Movement.X;

            var k = 1.0;

            if (y1 < -MaximumPlayerManhattanDistanceFromField)
                k = (-MaximumPlayerManhattanDistanceFromField - y0) / y1;
            if (y1 > FieldHeight + MaximumPlayerManhattanDistanceFromField)
                k = (FieldHeight + MaximumPlayerManhattanDistanceFromField - y0) / y1;
            if (x1 < -MaximumPlayerManhattanDistanceFromField)
                k = (-MaximumPlayerManhattanDistanceFromField - x0) / x1;
            if (x1 > FieldWidth + MaximumPlayerManhattanDistanceFromField)
                k = (FieldWidth + MaximumPlayerManhattanDistanceFromField - x0) / x1;

            player.Movement = new Vector(player.Movement.X * k, player.Movement.Y * k);
        }

        /// <summary>
        /// Updates the ball's movement in accordance with the specified actions.
        /// </summary>
        /// <param name="ai1Action">The first AI's action.</param>
        /// <param name="ai2Action">The second AI's action.</param>
        private void UpdateBall(ActionMessage ai1Action, ActionMessage ai2Action)
        {
            // add kick actions to players
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i].Kick.X = ai1Action?.PlayersActions[i].Kick.X ?? 0;
                GameState.FootballPlayers[i].Kick.Y = ai1Action?.PlayersActions[i].Kick.Y ?? 0;

                var kickX = GameState.FootballPlayers[i].Kick.X;
                var kickY = GameState.FootballPlayers[i].Kick.Y;

                if (double.IsNaN(kickX) || double.IsNaN(kickY) ||
                      double.IsInfinity(kickX) || double.IsInfinity(kickY))
                {
                    GameState.FootballPlayers[i].Kick = new Vector(0, 0);
                    if (NumberOfPlayer1Errors[SimulationErrorReason.InvalidKickVector]++ < MaximumNumberOfSameReasonErrorInLog)
                        MatchInfo.Errors.Add(new SimulationError()
                        {
                            Team = Team.FirstPlayer,
                            Time = CurrentTime,
                            AffectedPlayerNumber = i,
                            Reason = SimulationErrorReason.InvalidKickVector
                        });
                }

            }
            for (var i = 0; i < 11; i++)
            {
                GameState.FootballPlayers[i + 11].Kick.X = ai2Action?.PlayersActions[i].Kick.X ?? 0;
                GameState.FootballPlayers[i + 11].Kick.Y = ai2Action?.PlayersActions[i].Kick.Y ?? 0;


                var kickX = GameState.FootballPlayers[i + 11].Kick.X;
                var kickY = GameState.FootballPlayers[i + 11].Kick.Y;

                if (double.IsNaN(kickX) || double.IsNaN(kickY) ||
                      double.IsInfinity(kickX) || double.IsInfinity(kickY))
                {
                    GameState.FootballPlayers[i + 11].Kick = new Vector(0, 0);
                    if (NumberOfPlayer2Errors[SimulationErrorReason.InvalidKickVector]++ < MaximumNumberOfSameReasonErrorInLog)
                        MatchInfo.Errors.Add(new SimulationError()
                        {
                            Team = Team.SecondPlayer,
                            Time = CurrentTime,
                            AffectedPlayerNumber = i,
                            Reason = SimulationErrorReason.InvalidKickVector
                        });
                }
            }

            var playersNearBallKicking = GameState.FootballPlayers.Where(
                p => Vector.DistanceBetween(GameState.Ball.Position, p.Position) <= BallMaxDistanceForKick &&
                (Math.Abs(p.Kick.X) > 0.00000001 || Math.Abs(p.Kick.Y) > 0.00000001));

            var kickWinner = GetKickWinner(playersNearBallKicking.ToArray());

            // apply kick
            if (kickWinner != null)
            {
                // check whether the ball was shot or shot on target
                ProcessStoppedShots(kickWinner);

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
                            NumberOfPlayer1Errors[SimulationErrorReason.TooStrongKick]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Team = Team.FirstPlayer,
                                Time = CurrentTime,
                                AffectedPlayerNumber = kickWinner.Id,
                                Reason = SimulationErrorReason.TooStrongKick
                            });
                    }
                    else
                    {
                        if (newSpeed > maxAllowedSpeed * MinReportableCorrection &&
                            NumberOfPlayer2Errors[SimulationErrorReason.TooStrongKick]++ < MaximumNumberOfSameReasonErrorInLog)
                            MatchInfo.Errors.Add(new SimulationError()
                            {
                                Team = Team.SecondPlayer,
                                Time = CurrentTime,
                                AffectedPlayerNumber = kickWinner.Id - 11,
                                Reason = SimulationErrorReason.TooStrongKick
                            });
                    }
                }
            }

            // update ball position
            GameState.Ball.Position.X += GameState.Ball.Movement.X;
            GameState.Ball.Position.Y += GameState.Ball.Movement.Y;

            // ball deceleration (for the next step)
            var ratio = (GameState.Ball.CurrentSpeed - (BallDeceleration * StepInterval / 1000)) /
                GameState.Ball.CurrentSpeed;
            if (ratio < 0)
                ratio = 0;
            GameState.Ball.Movement.X *= (float)ratio;
            GameState.Ball.Movement.Y *= (float)ratio;
        }

        /// <summary>
        /// Processes an out if there is one currently happening.
        /// </summary>
        private void ProcessOut()
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
                    GameState.IsKickOff = true;

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
                            GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? Team.SecondPlayer : Team.FirstPlayer);

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
                    GameState.IsKickOff = true;

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
                           GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? Team.SecondPlayer : Team.FirstPlayer);

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
                GameState.IsKickOff = true;

                ball.Position.Y = 1;
                ball.Movement.X = 0f;
                ball.Movement.Y = 0f;

                var nearestPlayerFromOppositeTeam =
                          GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? Team.SecondPlayer : Team.FirstPlayer);

                nearestPlayerFromOppositeTeam.Position.X = ball.Position.X;
                nearestPlayerFromOppositeTeam.Position.Y = 0;
                nearestPlayerFromOppositeTeam.Movement.X = 0;
                nearestPlayerFromOppositeTeam.Movement.Y = 0;

                // push all opponent players aways from the kickoff position
                PushPlayersFromPosition(lastTeam, ball.Position);

            }
            if (ball.Position.Y > 75)
            {
                GameState.IsKickOff = true;

                ball.Position.Y = 74;
                ball.Movement.X = 0f;
                ball.Movement.Y = 0f;

                var nearestPlayerFromOppositeTeam =
                    GetNearestPlayerToBall(lastTeam == Team.FirstPlayer ? Team.SecondPlayer : Team.FirstPlayer);

                nearestPlayerFromOppositeTeam.Position.X = ball.Position.X;
                nearestPlayerFromOppositeTeam.Position.Y = 75;
                nearestPlayerFromOppositeTeam.Movement.X = 0;
                nearestPlayerFromOppositeTeam.Movement.Y = 0;

                // push all opponent players aways from the kickoff position
                PushPlayersFromPosition(lastTeam, ball.Position);
            }

        }

        /// <summary>
        /// Processes the goal if there is one currently happening.
        /// </summary>
        private void ProcessGoal()
        {
            var lastTeam = Team.FirstPlayer; // default (will be set correctly)
            var ball = GameState.Ball;

            for (var i = 0; i < 22; i++)
                if (LastKicker == GameState.FootballPlayers[i])
                    lastTeam = i < 11 ? Team.FirstPlayer : Team.SecondPlayer;

            if (ball.Position.X < 0 && ball.Position.Y < 75f / 2 + 7.32 / 2 && ball.Position.Y > 75f / 2 - 7.32 / 2)
            {
                GameState.IsKickOff = true;

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

                SetPlayersToStartingPositions(WhoIsOnLeft);
            }
            else if (ball.Position.X > 110 && ball.Position.Y < 75f / 2 + 7.32 / 2 && ball.Position.Y > 75f / 2 - 7.32 / 2)
            {
                GameState.IsKickOff = true;

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

                SetPlayersToStartingPositions(WhoIsOnLeft == Team.FirstPlayer ? Team.SecondPlayer : Team.FirstPlayer);
            }
        }

        /// <summary>
        /// Processes the stopped shots. Updates shots and shots on target statistics
        /// accordingly.
        /// </summary>
        /// <param name="currentKickWinner">The current kick winner.</param>
        private void ProcessStoppedShots(FootballPlayer currentKickWinner)
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

            var intersectionWithLeftGoalLine = GetIntersectionWithGoalLine(GameState.Ball, true);
            var intersectionWithRightGoalLine = GetIntersectionWithGoalLine(GameState.Ball, false);

            var wasBallGoingToGoalLine1 = intersectionWithLeftGoalLine != null;
            var wasBallGoingToGoalLine2 = intersectionWithRightGoalLine != null;
            var wasBallGoingToGoalPost1 = intersectionWithLeftGoalLine != null
                                          && intersectionWithLeftGoalLine.Y > 75f / 2 - 7.32 &&
                                          intersectionWithLeftGoalLine.Y < 75f / 2 + 7.32;
            var wasBallGoingToGoalPost2 = intersectionWithRightGoalLine != null
                                          && intersectionWithRightGoalLine.Y > 75f / 2 - 7.32 &&
                                          intersectionWithRightGoalLine.Y < 75f / 2 + 7.32;

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
        /// Pushes the players from the specified team away from the specified position in 
        /// accordance with the <see cref="MinimalOpponentDistanceFromKickoff"/>.
        /// Used for pushing opponent's away players from kickoffs.
        /// </summary>
        /// <param name="teamToBePushed">The team to be pushed.</param>
        /// <param name="position">The position from which the players will be pushed away.</param>
        private void PushPlayersFromPosition(Team teamToBePushed, Vector position)
        {
            var toBePushedPlayers = new List<FootballPlayer>();

            for (var i = 0; i < 11; i++)
            {
                var player = GameState.FootballPlayers[teamToBePushed == Team.FirstPlayer ? i : i + 11];
                if (Vector.DistanceBetween(player.Position, position) < MinimalOpponentDistanceFromKickoff)
                    toBePushedPlayers.Add(player);
            }

            foreach (var player in toBePushedPlayers)
            {
                var vectorToPlayer = new Vector(player.Position.X - position.X, player.Position.Y - position.Y);
                var length = vectorToPlayer.Length;

                // apply push
                vectorToPlayer.X *= MinimalOpponentDistanceFromKickoff / length;
                vectorToPlayer.Y *= MinimalOpponentDistanceFromKickoff / length;

                player.Position.X = position.X + vectorToPlayer.X;
                player.Position.Y = position.Y + vectorToPlayer.Y;

                if (player.Position.X > 110)
                    player.Position.X = 110 - MinimalOpponentDistanceFromKickoff;
                if (player.Position.Y > 75)
                    player.Position.Y = 75 - MinimalOpponentDistanceFromKickoff;
                if (player.Position.X < 0)
                    player.Position.X = MinimalOpponentDistanceFromKickoff;
                if (player.Position.Y < 0)
                    player.Position.Y = MinimalOpponentDistanceFromKickoff;
            }

        }

        /// <summary>
        /// Gets the nearest player to the ball from the specified team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns>Nearest <see cref="FootballPlayer"/> to the ball from the specified team.</returns>
        private FootballPlayer GetNearestPlayerToBall(Team team)
        {
            FootballPlayer nearestPlayer = null;
            var players = GameState.FootballPlayers;
            var ball = GameState.Ball;

            if (team == Team.FirstPlayer)
            {
                nearestPlayer = players[1];
                for (int i = 2; i < 11; i++)
                    if (Vector.DistanceBetween(ball.Position, players[i].Position) <
                        Vector.DistanceBetween(ball.Position, nearestPlayer.Position))
                        nearestPlayer = players[i];
            }
            if (team == Team.SecondPlayer)
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
        /// Gets the intersection that the ball would have with the goal line if it wasn't stopped by any player.
        /// </summary>
        /// <param name="ball">The ball.</param>
        /// <param name="leftGoalLine">If set to true then the intersection with the left goal line is
        /// retrieved; otherwise the intersection with the right goal line is retrieved.</param>
        /// <returns>The intersection with the goal line point. Null if there is no such intersection (ball 
        /// is not moving to the goal line or it is moving too slow).</returns>
        private static Vector GetIntersectionWithGoalLine(Ball ball, bool leftGoalLine)
        {
            double ballMovementToIntersectionVectorRatio;
            if (Math.Abs(ball.Movement.X) < 0.0001) // doesn't move
                return null;

            if (leftGoalLine)
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
            var discriminant = 4 * Math.Pow(ball.CurrentSpeed, 2) - 8 * BallDeceleration * distanceFromIntersection;
            if (discriminant < 0)
                return null; // ball will stop -> no intersection reached

            var time = (ball.CurrentSpeed - Math.Sqrt(discriminant)) / BallDeceleration;

            // final speed = v + a * t
            var speedAtIntersection = ball.CurrentSpeed + BallDeceleration * time;

            // is speed higher than minimal shot speed (4 for now)
            if (speedAtIntersection <= 4)
                return null;

            return intersection;
        }

        /// <summary>
        /// Gets the kick winner in accordance with the kickers' possession parameters.
        /// </summary>
        /// <param name="kickers">The football players that are currently kicking the ball.</param>
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