using System.Windows.Forms;

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
        }




       
    }
}
