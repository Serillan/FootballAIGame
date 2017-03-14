using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FootballAIGame.LocalDesktopSimulator.CustomControls;
using FootballAIGame.LocalSimulationBase.Models;

namespace FootballAIGame.LocalDesktopSimulator
{
    class MatchPlayer
    {
        private Match Match { get; set; }

        private Panel GamePanel { get; set; }

        private Label ScoreLabel { get; set; }

        private Label TimeLabel { get; set; }

        private Slider PlaySlider { get; set; }

        public bool IsPlaying { get; private set; }

        public double Speed { get; set; } = 1;

        private double UpdateFrequency => 200 / Speed; // [ms]

        public MatchPlayer(Panel gamePanel, Label scoreLabel, Label timeLabel, Slider playSlider)
        {
            GamePanel = gamePanel;
            ScoreLabel = scoreLabel;
            TimeLabel = timeLabel;
            PlaySlider = playSlider;

            GamePanel.Paint += GamePanelOnPaint;
        }

        private void GamePanelOnPaint(object sender, PaintEventArgs paintEventArgs)
        {
            var graphics = paintEventArgs.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            graphics.ScaleTransform(GamePanel.Width / 120f, GamePanel.Height / 85f);
            graphics.TranslateTransform(5, 5);
            // todo line width

            graphics.Clear(Color.White);

            DrawField(graphics);

            if (Match != null)
            {
                DrawBall();
                DrawPlayers();
            }

            graphics.Flush();
        }

        public void StartPlaying(Match match)
        {
            lock (this)
            {
                if (IsPlaying)
                    return;

                IsPlaying = true;
            }

            Match = match;

            Task.Run(() => PlayMatch());
        }

        private void PlayMatch()
        {
            long lastUpdateTime = 0;

            while (true)
            {
                if (!IsPlaying)
                    return;

                var delta = (Stopwatch.GetTimestamp() - lastUpdateTime) * 1000.0 / Stopwatch.Frequency; // [ms]

                if (!(delta > UpdateFrequency)) continue;

                lastUpdateTime = Stopwatch.GetTimestamp();

                GamePanel.BeginInvoke((MethodInvoker)(() =>
                {
                    if (++PlaySlider.Value >= PlaySlider.Max)
                    {
                        PlaySlider.Value = PlaySlider.Max;
                        StopPlaying();
                    }

                    UpdateStatus();
                }));
            }
        }

        private void UpdateStatus()
        {
            
        }

        private void DrawPlayers()
        {
        }

        private void DrawBall()
        {
        }

        private void DrawField(Graphics graphics)
        {
            var grassBrush = new SolidBrush(Color.FromArgb(255, 1, 166, 17));

            graphics.FillRectangle(grassBrush, new Rectangle(-5, -5, 120, 85));

            var pen = new Pen(Color.White, 1 * 120f / GamePanel.Width);

            // touch and goal lines
            graphics.DrawRectangle(pen, 0, 0, 110, 75);

            // mid line
            graphics.DrawLine(pen, 55, 0, 55, 75);

            // mid circle
            graphics.DrawEllipse(pen, 55 - 9.15f, 37.5f - 9.15f, 2 * 9.15f, 2 * 9.15f);



        }

        public void StopPlaying()
        {
            IsPlaying = false;
        }

    }
}
