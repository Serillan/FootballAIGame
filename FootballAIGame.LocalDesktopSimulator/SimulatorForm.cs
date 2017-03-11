using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FootballAIGame.LocalSimulationBase;
using FootballAIGame.MatchSimulation;

namespace FootballAIGame.LocalDesktopSimulator
{
    public partial class SimulatorForm : Form
    {
        public SimulatorForm()
        {
            InitializeComponent();
            DoCustomInitialization();
        }

        private void DoCustomInitialization()
        {
            SpeedDropDownList.SelectedItem = SpeedDropDownList.Items[0];

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

        private async void StartMatchButtonClick(object sender, EventArgs e)
        {
            if (AiListBox.SelectedItems.Count != 2)
            {
                MessageBox.Show(this, "Invalid number of AI selected. Select 2 AI.");
                return;
            }

            var ai1 = AiListBox.Items[0].ToString();
            var ai2 = AiListBox.Items[1].ToString();

            PlayButton.Enabled = false;
            RestartButton.Enabled = false;

            SimulationLabel.Visible = true;
            SimulationProgress.Visible = true;

            StartMatchButton.Text = "Stop match";
            StartMatchButton.Click += StopMatchButtonClick;
            StartMatchButton.Click -= StartMatchButtonClick;
            StartMatchButton.Invalidate();

            var match = await SimulationManager.Instance.Simulate(ai1, ai2);

            StartMatchButton.Text = "Start match";
            StartMatchButton.Click += StopMatchButtonClick;
            StartMatchButton.Click -= StartMatchButtonClick;

            PlayButton.Enabled = true;
            RestartButton.Enabled = true;

            SimulationLabel.Visible = false;
            SimulationProgress.Visible = false;
        }

        private async void StopMatchButtonClick(object sender, EventArgs e)
        {
            await Task.Yield();
        }

    }
}
