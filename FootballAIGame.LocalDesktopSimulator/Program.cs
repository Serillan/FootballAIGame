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
        /// The default listening port. This port is used for listening if the user
        /// doesn't provide his own port in the program's argument.
        /// </summary>
        private const int DefaultListeningPort = 50030;

        /// <summary>
        /// The entry point for the application. Starts the winforms application. Starts the listening for AI
        /// connections.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            int port = DefaultListeningPort;

            // the port can be specified in the first program's argument (options are ignored)
            foreach (var arg in args)
            {
                if (!arg.StartsWith("-"))
                {
                    if (!int.TryParse(arg, out port))
                    {
                        MessageBox.Show(null, $"Invalid specified port: {arg}", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    break;
                }
            }

            var listeningTask = SimulationManager.Instance.StartAcceptingConnectionsAsync(port);

            if (listeningTask.IsCompleted) // address already in use
            {
                MessageBox.Show(null, $"Error: The port { port} is already being used by another process. " +
                    $"You might already have another simulator on.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Environment.Exit(1);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimulatorForm());
        }
    }
}
