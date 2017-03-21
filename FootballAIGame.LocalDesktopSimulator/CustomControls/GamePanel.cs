using System.Windows.Forms;

namespace FootballAIGame.LocalDesktopSimulator.CustomControls
{
    /// <summary>
    /// Represents the <see cref="Panel"/> used for drawing games.
    /// It is double buffered and redrawn during resizing.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Panel" />
    sealed class GamePanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GamePanel"/> class.
        /// </summary>
        public GamePanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }
}
