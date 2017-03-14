using System;
using System.Drawing;
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
    public partial class SimulatorForm : Form
    {
        private const int ProgressBarUpdateInterval = 100;

        private Match LoadedMatch { get; set; }

        private MatchPlayer MatchPlayer { get; set; }

        public SimulatorForm()
        {
            InitializeComponent();
            DoCustomInitialization();
        }

        private void DoCustomInitialization()
        {
            SpeedDropDownList.SelectedItem = SpeedDropDownList.Items[0];
            MatchPlayer = new MatchPlayer(SimulationPanel, CurrentScoreLabel, CurrentTimeLabel, PlaySlider);

            ConnectionManager.Instance.PlayerConnectedHandler += PlayerConnectedHandler;
            ConnectionManager.Instance.PlayerDisconectedHandler += PlayerDisconectedHandler;

        }

        private async Task PlayerConnectedHandler(ClientConnection connection)
        {
            await Task.Yield();
            UpdateAiList();
        }

        private async Task PlayerDisconectedHandler(ClientConnection connection)
        {
            await Task.Yield();
            UpdateAiList();
        }

        private void UpdateAiList()
        {
            AiListBox.BeginInvoke((MethodInvoker) (() =>
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

        private async void StartMatchButtonClick(object sender, EventArgs e)
        {
            if (AiListBox.SelectedItems.Count != 2)
            {
                MessageBox.Show(this, "Invalid number of AI selected. Select 2 AI.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ai1 = AiListBox.Items[0].ToString();
            var ai2 = AiListBox.Items[1].ToString();

            PlayButton.Enabled = false;
            RestartButton.Enabled = false;
            SaveMatchToolStripMenuItem.Enabled = false;
            LoadMatchToolStripMenuItem.Enabled = false;
            MatchPlayer.StopPlaying();

            SimulationLabel.Visible = true;
            SimulationProgress.Visible = true;

            StartMatchButton.Text = "Stop match";
            StartMatchButton.Click += StopMatchButtonClick;
            StartMatchButton.Click -= StartMatchButtonClick;

            try
            {
                var simulationTask = SimulationManager.Instance.Simulate(ai1, ai2);

                var progressBarUpdateCancelleration = new CancellationTokenSource();
                var progressBarUpdatingTask = StartUpdatingProgressBarAsync(ai1, ai2, progressBarUpdateCancelleration.Token);

                var match = await simulationTask;

                progressBarUpdateCancelleration.Cancel();
                await progressBarUpdatingTask;

                // todo prepare match for watching
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

        private async Task StartUpdatingProgressBarAsync(string ai1, string ai2, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var step = 0;

                if (SimulationManager.Instance.TryGetSimulationStep(ai1, ai2, out step))
                {
                    SimulationProgress.Value = step;
                }

                await Task.Delay(ProgressBarUpdateInterval);
            }
        }

        private void LoadMatch(Match match)
        {
            var matchInfo = match.MatchInfo;

            FinalShotsLabel.Text = $"{matchInfo.Team1Statistics.Shots} / {matchInfo.Team2Statistics.Shots}";
            FinalScoreLabel.Text = $"{matchInfo.Team1Statistics.Goals} - {matchInfo.Team2Statistics.Goals}";
            FinalShotsOnTargetLabel.Text =
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

            LoadedMatch = match;

            PlayButton.Enabled = true;
            RestartButton.Enabled = true;
            PlaySlider.Value = 0;
        }

        private static string GetErrorMessage(SimulationError error, string ai1, string ai2)
        {
            var ai = error.Team == Team.FirstPlayer ? ai1 : ai2;

            switch (error.Type)
            {
                case SimulationError.ErrorType.TooHighSpeed:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has too high speed.";
                case SimulationError.ErrorType.TooHighAcceleration:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has too high acceleration.";
                case SimulationError.ErrorType.TooStrongKick:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has made too strong kick.";
                case SimulationError.ErrorType.InvalidMovementVector:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has invalid movement vector set.";
                case SimulationError.ErrorType.InvalidKickVector:
                    return $"{error.Time} : {ai} - Player{error.AffectedPlayerNumber} has invalid kick vector set.";
                case SimulationError.ErrorType.Disconnection:
                    return $"{error.Time} : {ai} - Player has disconnected.";
                case SimulationError.ErrorType.Cancel:
                    return $"{error.Time} : {ai} - Player has left the match.";
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private async void StopMatchButtonClick(object sender, EventArgs e)
        {
            await Task.Yield();
        }

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

            Stream fileStream;

            try
            {
                fileStream = dialog.OpenFile();
            }
            catch (Exception)
            {
                return;
            }

            LoadedMatch.Save(fileStream);

            fileStream.Close();
        }

        private void LoadMatchToolStripMenuItemClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.ShowDialog(this);

            Stream fileStream = null;

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


            fileStream?.Close();
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            if (!MatchPlayer.IsPlaying)
                MatchPlayer.StartPlaying(LoadedMatch);
            else
                MatchPlayer.StopPlaying();
        }

        private void SpeedDropDownListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (MatchPlayer != null)
            {
                MatchPlayer.Speed = Convert.ToDouble(((string) SpeedDropDownList.SelectedItem).Substring(0, 1));
            }
        }

        private void RestartButtonClick(object sender, EventArgs e)
        {
            PlaySlider.Value = 0;
        }

    }
}
