using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FootballAIGame.LocalDesktopSimulator.CustomControls
{
    /// <summary>
    /// Represents the slider that can be used for a video player.
    /// Consist of the disk that can be moved horizontally in the given rectangle.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.UserControl" />
    public partial class Slider : UserControl
    {
        /// <summary>
        /// The current value of the slider.
        /// </summary>
        private int _value;

        /// <summary>
        /// The minimum allowed value of the slider.
        /// </summary>
        private int _min;

        /// <summary>
        /// The maximum allowed value of the slider.
        /// </summary>
        private int _max = 100;

        /// <summary>
        /// Gets or sets a value indicating whether the dragging of the circle is happening.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dragging is dragging; otherwise, <c>false</c>.
        /// </value>
        private bool IsDragging { get; set; }

        /// <summary>
        /// Gets the percentage of the current progress.
        /// </summary>
        /// <value>
        /// The percentage of the current progress.
        /// </value>
        private float Percentage => (float)Value / (Max - Min);

        /// <summary>
        /// Gets the draggable circle radius.
        /// </summary>
        /// <value>
        /// The circle radius.
        /// </value>
        private float CircleRadius => ClientRectangle.Height / 2.0f;

        /// <summary>
        /// Gets the draggable circle center x-coordinate.
        /// </summary>
        /// <value>
        /// The circle center x-coordinate.
        /// </value>
        private float CircleCenterX => Percentage * UsableWidth + CircleRadius;

        /// <summary>
        /// Gets the draggable circle center y-coordinate.
        /// </summary>
        /// <value>
        /// The circle center y-coordinate.
        /// </value>
        private float CircleCenterY => ClientRectangle.Height / 2.0f;

        /// <summary>
        /// Gets the width of the area to which the center of the draggable circle can be placed.
        /// </summary>
        /// <value>
        /// The width of the usable area.
        /// </value>
        private float UsableWidth => ClientRectangle.Width - 2 * CircleRadius;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [Category("Slider")]
        [DefaultValue(0)]
        public int Value    
        {
            get { return _value; }
            set { _value = value; Invalidate(); }
        }

        /// <summary>
        /// Gets or sets the minimum allowed value.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        [Category("Slider")]
        [DefaultValue(0)]
        public int Min
        {
            get { return _min; }
            set { _min = value; Invalidate(); }
        }

        /// <summary>
        /// Gets or sets the maximum allowed value.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        [Category("Slider")]
        [DefaultValue(100)]
        public int Max
        {
            get { return _max; }
            set { _max = value; Invalidate(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Slider"/> class.
        /// </summary>
        public Slider()
        {
            // designer
            InitializeComponent();

            ResizeRedraw = true;

            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
        }

        /// <summary>
        /// Occurs when the mouse is released after the button was pressed on the this instance client area.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseEventArgs">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            IsDragging = false;
        }

        /// <summary>
        /// Occurs when the mouse is moved while being captured by this instance. Updates
        /// the current <see cref="Value"/> accordingly.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseEventArgs">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (IsDragging)
                AdjustValue(mouseEventArgs.X);
        }

        /// <summary>
        /// Occurs when the mouse is released after being captured by this instance.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseEventArgs">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            IsDragging = true;

            if (!IsInCircle(mouseEventArgs.X, mouseEventArgs.Y))
            {
                AdjustValue(mouseEventArgs.X);
            }
        }

        /// <summary>
        /// Occurs when this instance is redrawn.
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
        /// Draws the current progress.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            base.OnPaint(e);
            DrawCurrentProgress(e.Graphics);
            e.Graphics.Flush();
        }

        /// <summary>
        /// Adjusts the value in accordance with the specified mouse position.
        /// </summary>
        /// <param name="mouseX">The mouse x-coordinate relative to this instance client area.</param>
        private void AdjustValue(int mouseX)
        {
            var xInUsableArea = Math.Min(Math.Max(0, mouseX - CircleRadius), UsableWidth);
            var newPercentage = xInUsableArea/UsableWidth;

            Value = Min + (int)Math.Round(newPercentage*(Max - Min));
        }

        /// <summary>
        /// Determines whether the specified position is in the draggable circle.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>
        ///   <c>true</c> if the specified position is in the draggable circle; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInCircle(int x, int y)
        {
            return Math.Sqrt(Math.Pow(x - CircleCenterX, 2) + Math.Pow(y - CircleCenterY, 2)) <= CircleRadius;
        }

        /// <summary>
        /// Draws the current progress in accordance to the <see cref="Value"/>.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        private void DrawCurrentProgress(Graphics graphics)
        {
            if (Max-Min == 0)
                return;

            graphics.FillEllipse(new SolidBrush(ForeColor), CircleCenterX - CircleRadius, 
                CircleCenterY - CircleRadius, 2*CircleRadius, 2*CircleRadius);
        }

    }
}
