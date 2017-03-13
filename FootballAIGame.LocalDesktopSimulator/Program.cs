using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using FootballAIGame.LocalSimulationBase;
using FootballAIGame.MatchSimulation;

namespace FootballAIGame.LocalDesktopSimulator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Task.Run(() => SimulationManager.Instance.StartAcceptingConnections());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimulatorForm());

        }
    }
}
