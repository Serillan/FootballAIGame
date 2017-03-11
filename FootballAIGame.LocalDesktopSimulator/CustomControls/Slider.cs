using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FootballAIGame.LocalDesktopSimulator.CustomControls
{
    public partial class Slider : UserControl
    {
        [Category("Slider")]
        [DefaultValue(0)]
        public int Value { get; set; } = 0;

        [Category("Slider")]
        [DefaultValue(0)]
        public int Min { get; set; } = 0;

        [Category("Slider")]
        [DefaultValue(100)]
        public int Max { get; set; } = 100;

        public Slider()
        {
            // designer
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            base.OnPaint(e);
            DrawCurrentProgress(e.Graphics);
            e.Graphics.Flush();
        }

        private void DrawCurrentProgress(Graphics g)
        {
            if (Max-Min == 0)
                return;

            var percentage = (float)Value/(Max - Min);

            var centerX = percentage*ClientRectangle.Width;
            var centerY = ClientRectangle.Height/2f;
            var radius = ClientRectangle.Height/2f;

            g.FillEllipse(new SolidBrush(ForeColor), centerX - radius, centerY - radius, 2*radius, 2*radius);
        }

    }
}
