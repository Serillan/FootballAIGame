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
        static void Main()
        {
            Task.Run(() => SimulationManager.Instance.StartAcceptingConnectionsAsync());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimulatorForm());

        }
    }
}
