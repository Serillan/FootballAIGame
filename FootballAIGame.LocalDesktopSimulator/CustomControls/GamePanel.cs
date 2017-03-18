using System.Windows.Forms;

namespace FootballAIGame.LocalDesktopSimulator.CustomControls
{
    sealed class GamePanel : Panel
    {
        public GamePanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }


    }
}
