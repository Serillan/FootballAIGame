using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using FootballAIGame.LocalSimulationBase;

namespace FootballAIGame.LocalDesktopSimulator
{
    /// <summary>
    /// Provides entry point of the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The entry point for the application. Starts the winforms application. Starts the listening for AI
        /// connections.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var listeningTask = SimulationManager.Instance.StartAcceptingConnectionsAsync();

            if (listeningTask.IsCompleted) // address already in use
            {
                MessageBox.Show(null, "The listening address is already being used.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimulatorForm());
        }
    }
}
