using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
