using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FootballAIGame.LocalDesktopSimulator.CustomControls
{
    public partial class Slider : UserControl
    {
        private int _value;
        private int _min;
        private int _max = 100;

        private bool IsDragging { get; set; }

        private float Percentage => (float)Value / (Max - Min);

        private float CircleRadius => ClientRectangle.Height / 2.0f;

        private float CircleCenterX => Percentage * UsableWidth + CircleRadius;
        private float CircleCenterY => ClientRectangle.Height / 2.0f;

        private float UsableWidth => ClientRectangle.Width - 2 * CircleRadius;

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

            ResizeRedraw = true;

            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
        }

        private void OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            IsDragging = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (IsDragging)
                AdjustValue(mouseEventArgs.X);
        }

        private void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            IsDragging = true;

            if (!IsInCircle(mouseEventArgs.X, mouseEventArgs.Y))
            {
                AdjustValue(mouseEventArgs.X);
            }
        }

        private void AdjustValue(int mouseX)
        {
            var xInUsableArea = Math.Min(Math.Max(0, mouseX - CircleRadius), UsableWidth);
            var newPercentage = xInUsableArea/UsableWidth;

            Value = Min + (int)Math.Round(newPercentage*(Max - Min));
        }

        private bool IsInCircle(int x, int y)
        {
            return Math.Sqrt(Math.Pow(x - CircleCenterX, 2) + Math.Pow(y - CircleCenterY, 2)) <= CircleRadius;
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

            g.FillEllipse(new SolidBrush(ForeColor), CircleCenterX - CircleRadius, 
                CircleCenterY - CircleRadius, 2*CircleRadius, 2*CircleRadius);
        }

    }
}
