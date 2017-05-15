using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FootballAIGame.LocalSimulationBase;
using FootballAIGame.LocalSimulationBase.Models;
using FootballAIGame.MatchSimulation;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.LocalDesktopSimulator
{
    /// <summary>
    /// Represents a window that makes up an application's user interface.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    partial class SimulatorForm : Form
    {
        /// <summary>
        /// The progress bar update interval in which the progress bar will be updated during
        /// a match simulation.
        /// </summary>
        private const int ProgressBarUpdateInterval = 100;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MatchPlayer"/> was playing the playback
        /// of the match before the mouse down happened.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="MatchPlayer"/> was playing the match before 
        /// the mouse down happened; otherwise, <c>false</c>.
        /// </value>
        private bool WasPlayingBeforeMouseDownOnSlider { get; set; }

        /// <summary>
        /// Gets or sets the loaded match.
        /// </summary>
        /// <value>
        /// The loaded match.
        /// </value>
        private Match LoadedMatch { get; set; }

        /// <summary>
        /// Gets or sets the match player used for playing the playbacks of the matches.
        /// </summary>
        /// <value>
        /// The match player.
        /// </value>
        private MatchPlayer MatchPlayer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatorForm" /> class.
        /// </summary>
        public SimulatorForm()
        {
            InitializeComponent();
            DoCustomInitialization();
        }

        /// <summary>
        /// Gets the error message corresponding to the specified <see cref="SimulationError"/>.
        /// </summary>
        /// <param name="error">The simulation error.</param>
        /// <param name="ai1">The first AI from the simulation.</param>
        /// <param name="ai2">The second AI from the simulation.</param>
        /// <returns>The error message corresponding to the specified error.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="error"/> doesn't have
        /// a corresponding error message.</exception>
        private static string GetErrorMessage(SimulationError error, string ai1, string ai2)
        {
            var ai = error.Team == Team.FirstPlayer ? ai1 : ai2;

            switch (error.Reason)
            {
                case SimulationErrorReason.TooHighSpeed:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has too high speed.";
                case SimulationErrorReason.TooHighAcceleration:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has too high acceleration.";
                case SimulationErrorReason.TooStrongKick:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has made too strong kick.";
                case SimulationErrorReason.InvalidMovementVector:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has invalid movement vector set.";
                case SimulationErrorReason.InvalidKickVector:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has invalid kick vector set.";
                case SimulationErrorReason.InvalidParameters:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has invalid parameters.";
                case SimulationErrorReason.GetParametersTimeout:
                    return $"{error.Time} : {ai} - Get parameters request timeout.";
                case SimulationErrorReason.GetActionTimeout:
                    return $"{error.Time} : {ai} - Get action request timeout.";
                case SimulationErrorReason.Disconnection:
                    return $"{error.Time} : {ai} - Player has disconnected.";
                case SimulationErrorReason.Cancellation:
                    return $"{error.Time} : {ai} - Player has left the match.";
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        /// <summary>
        /// Does the custom (non-designer) initialization of this instance.
        /// </summary>
        private void DoCustomInitialization()
        {
            SpeedDropDownList.SelectedItem = SpeedDropDownList.Items[0];
            MatchPlayer = new MatchPlayer(MatchPanel, CurrentScoreLabel, CurrentTimeLabel, PlaySlider);

            ConnectionManager.Instance.ClientLoggedInHandler += HandlePlayerConnectionAsync;
            ConnectionManager.Instance.ActiveClientDisconnectedHandler += HandlePlayerDisconnectionAsync;

            PlaySlider.MouseDown += PlaySliderOnMouseDown;
            PlaySlider.MouseUp += PlaySliderOnMouseUp;

            // if someone connected before the handler was set
            AiListBox.Items.AddRange(ConnectionManager.Instance.ActiveConnections.Select(c => c.AiName).Cast<object>().ToArray());
        }

        /// <summary>
        /// Occurs when the mouse button is released after the button was pressed on the <see cref="PlaySlider"/>.
        /// Updates <see cref="MatchPlayer"/> accordingly to play the right part of the match.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseEventArgs">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PlaySliderOnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            MatchPlayer.RenderCurrentState();

            if (WasPlayingBeforeMouseDownOnSlider)
                MatchPlayer.StartPlaying();
        }

        /// <summary>
        /// Occurs when the mouse is pressed on the <see cref="PlaySlider"/>.
        /// Stops the <see cref="MatchPlayer"/> from playing the match.
        /// Updates <see cref="WasPlayingBeforeMouseDownOnSlider"/> accordingly.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseEventArgs">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PlaySliderOnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            WasPlayingBeforeMouseDownOnSlider = MatchPlayer.IsPlaying;
            if (MatchPlayer.IsPlaying)
                MatchPlayer.StopPlaying();
        }

        /// <summary>
        /// Handles the player connection asynchronously.
        /// Occurs when the client connects.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task HandlePlayerConnectionAsync(ClientConnection connection)
        {
            await Task.Yield();
            UpdateAiList();
        }

        /// <summary>
        /// Handles the player disconnection asynchronously.
        /// Occurs when the client disconnects.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task HandlePlayerDisconnectionAsync(ClientConnection connection)
        {
            await Task.Yield();
            UpdateAiList();
        }

        /// <summary>
        /// Updates the AI list to currently active client connections.
        /// </summary>
        private void UpdateAiList()
        {
            AiListBox.BeginInvoke((MethodInvoker)(() =>
           {
               lock (ConnectionManager.Instance.ActiveConnections)
               {
                   AiListBox.Items.Clear();
                   AiListBox.Items.AddRange(ConnectionManager.Instance.ActiveConnections
                       .Select(c => c.AiName).Cast<object>().ToArray());
               }
               AiListBox.Invalidate();
           }));
        }

        /// <summary>
        /// Occurs when the <see cref="StartMatchButton"/> is clicked. Starts the match simulation between
        /// the selected AIs from the <see cref="AiListBox"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void StartMatchButtonClick(object sender, EventArgs e)
        {
            if (AiListBox.SelectedItems.Count != 2)
            {
                MessageBox.Show(this, "Invalid number of AI selected. Select 2 AI.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ai1 = AiListBox.SelectedItems[0].ToString();
            var ai2 = AiListBox.SelectedItems[1].ToString();

            PlayButton.Enabled = false;
            RestartButton.Enabled = false;
            SaveMatchToolStripMenuItem.Enabled = false;
            LoadMatchToolStripMenuItem.Enabled = false;
            MatchPlayer.StopPlaying();

            SimulationLabel.Visible = true;
            SimulationProgress.Visible = true;
            FirstAiLabel.Visible = true;
            SecondAiLabel.Visible = true;
            VsLabel.Visible = true;

            FirstAiLabel.Text = ai1;
            SecondAiLabel.Text = ai2;

            StartMatchButton.Text = "Stop match";
            StartMatchButton.Click += StopMatchButtonClick;
            StartMatchButton.Click -= StartMatchButtonClick;

            try
            {
                // run on thread pool, not on UI thread !!!
                var simulationTask = Task.Run(async () => await SimulationManager.Instance.SimulateAsync(ai1, ai2));

                var progressBarUpdateCancelleration = new CancellationTokenSource();
                var progressBarUpdatingTask = StartUpdatingProgressBarAsync(ai1, ai2, progressBarUpdateCancelleration.Token);

                var match = await simulationTask;

                progressBarUpdateCancelleration.Cancel();
                await progressBarUpdatingTask;

                LoadMatch(match);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            StartMatchButton.Text = "Start match";
            StartMatchButton.Click += StartMatchButtonClick;
            StartMatchButton.Click -= StopMatchButtonClick;

            if (LoadedMatch != null)
            {
                PlayButton.Enabled = true;
                RestartButton.Enabled = true;
            }

            LoadMatchToolStripMenuItem.Enabled = true;
            SaveMatchToolStripMenuItem.Enabled = true;

            SimulationLabel.Visible = false;
            SimulationProgress.Visible = false;
        }

        /// <summary>
        /// Starts to update progress bar asynchronously in accordance with <see cref="ProgressBarUpdateInterval"/>.
        /// Progress bar shows the current progress in the currently simulated match.
        /// </summary>
        /// <param name="ai1">The name of the first AI that is currently in the simulated match.</param>
        /// <param name="ai2">The name of the second AI that is currently in the simulated match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task StartUpdatingProgressBarAsync(string ai1, string ai2, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int step;

                if (SimulationManager.Instance.TryGetSimulationStep(ai1, ai2, out step))
                {
                    SimulationProgress.Value = step;
                }

                await Task.Delay(ProgressBarUpdateInterval);
            }
        }

        /// <summary>
        /// Loads the match. Updates the form to show all the information about the specified <see cref="Match"/>.
        /// </summary>
        /// <param name="match">The match.</param>
        private void LoadMatch(Match match)
        {
            var matchInfo = match.MatchInfo;

            ShotsLabel.Text = $"{matchInfo.Team1Statistics.Shots} / {matchInfo.Team2Statistics.Shots}";
            ScoreLabel.Text = $"{matchInfo.Team1Statistics.Goals} - {matchInfo.Team2Statistics.Goals}";
            ShotsOnTargetLabel.Text =
                $"{matchInfo.Team1Statistics.ShotsOnTarget} / {matchInfo.Team2Statistics.ShotsOnTarget}";

            var goalsEnumerable = from goal in matchInfo.Goals
                                  let aiName = goal.TeamThatScored == Team.FirstPlayer ? match.Ai1Name : match.Ai2Name
                                  select $"{goal.ScoreTime} : {aiName} - Player{goal.ScorerNumber}";

            GoalsListBox.Items.Clear();
            GoalsListBox.Items.AddRange(goalsEnumerable.Cast<object>().ToArray());

            ErrorsListBox.Items.Clear();
            ErrorsListBox.Items.AddRange(matchInfo.Errors
                .Select(e => GetErrorMessage(e, match.Ai1Name, match.Ai2Name))
                .Cast<object>()
                .ToArray());

            CurrentScoreLabel.Text = "0:0";
            CurrentTimeLabel.Text = "0:0";
            PlaySlider.Value = 0;

            FirstAiLabel.Text = match.Ai1Name;
            SecondAiLabel.Text = match.Ai2Name;

            LoadedMatch = match;
            MatchPlayer.LoadMatch(match);

            PlayButton.Enabled = true;
            RestartButton.Enabled = true;
            FirstAiLabel.Visible = true;
            SecondAiLabel.Visible = true;
            VsLabel.Visible = true;
            PlaySlider.Value = 0;
        }

        /// <summary>
        /// Occurs when the stop match button is clicked.
        /// Stops the ongoing match simulation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void StopMatchButtonClick(object sender, EventArgs e)
        {
            var ai1 = AiListBox.Items[0].ToString();
            var ai2 = AiListBox.Items[1].ToString();

            SimulationManager.Instance.StopSimulation(ai1, ai2);
        }

        /// <summary>
        /// Occurs when the <see cref="SaveMatchToolStripMenuItem"/> is clicked.
        /// Opens the dialog to select the file to which afterwards the <see cref="LoadedMatch"/> is saved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SaveMatchToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (LoadedMatch == null)
            {
                MessageBox.Show(this, "There is no loaded match.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dialog = new SaveFileDialog();

            dialog.ShowDialog(this);

            try
            {
                var fileStream = dialog.OpenFile();
                LoadedMatch.Save(fileStream);
                fileStream.Close();
            }
            catch (Exception)
            {
                // ignore (in case of cancel, incorrect file etc.)
            }
        }

        /// <summary>
        /// Occurs when the <see cref="LoadMatchToolStripMenuItem"/> is clicked.
        /// Opens the dialog to select the file from which afterwards the <see cref="Match"/> is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LoadMatchToolStripMenuItemClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.ShowDialog(this);

            Stream fileStream;

            try
            {
                fileStream = dialog.OpenFile();
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                var match = Match.Load(fileStream);
                MatchPlayer.StopPlaying();
                LoadMatch(match);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Couldn't load the match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            fileStream.Close();
        }

        /// <summary>
        /// Occurs when the <see cref="PlayButton"/> is clicked.
        /// Starts playing playback of <see cref="LoadedMatch"/> using <see cref="MatchPlayer"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void PlayButtonClick(object sender, EventArgs e)
        {
            if (!MatchPlayer.IsPlaying)
                MatchPlayer.StartPlaying();
            else
                MatchPlayer.StopPlaying();
        }

        /// <summary>
        /// Occurs when the <see cref="SpeedDropDownList"/> selected value is changed.
        /// Updates <see cref="MatchPlayer"/> accordingly to adjust the playing speed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SpeedDropDownListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (MatchPlayer != null)
            {
                MatchPlayer.Speed = Convert.ToDouble(((string)SpeedDropDownList.SelectedItem).Substring(0, 1));
            }
        }

        /// <summary>
        /// Occurs when the <see cref="RestartButton"/> is clicked.
        /// Changes <see cref="PlaySlider"/> value to 0.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RestartButtonClick(object sender, EventArgs e)
        {
            PlaySlider.Value = 0;
            MatchPlayer.RenderCurrentState();
        }
    }
}
