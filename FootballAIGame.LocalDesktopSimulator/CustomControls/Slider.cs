using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FootballAIGame.LocalDesktopSimulator.CustomControls
{
    public partial class Slider : UserControl
    {
        private int _value = 0;
        private int _min = 0;
        private int _max = 100;

        [Category("Slider")]
        [DefaultValue(0)]
        public int Value    
        {
            get { return _value; }
            set { _value = value; Invalidate(); }
        }

        [Category("Slider")]
        [DefaultValue(0)]
        public int Min
        {
            get { return _min; }
            set { _min = value; Invalidate(); }
        }

        [Category("Slider")]
        [DefaultValue(100)]
        public int Max
        {
            get { return _max; }
            set { _max = value; Invalidate(); }
        }

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

            var radius = ClientRectangle.Height / 2f;

            var percentage = (float)Value/(Max - Min);
            var usableWidth = ClientRectangle.Width - 2*radius;

            var centerX = percentage*usableWidth + radius;
            var centerY = ClientRectangle.Height/2f;

            g.FillEllipse(new SolidBrush(ForeColor), centerX - radius, centerY - radius, 2*radius, 2*radius);
        }

    }
}
