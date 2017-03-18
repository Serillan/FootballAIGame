using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FootballAIGame.LocalDesktopSimulator.CustomControls;
using FootballAIGame.LocalSimulationBase.Models;
using FootballAIGame.MatchSimulation.Models;

namespace FootballAIGame.LocalDesktopSimulator
{
    class MatchPlayer
    {
        private const int DefaultUpdateFrequency = 200;

        private Match Match { get; set; }

        private Panel GamePanel { get; set; }

        private Label ScoreLabel { get; set; }

        private Label TimeLabel { get; set; }

        private Slider PlaySlider { get; set; }

        public bool IsPlaying { get; private set; }

        public double Speed { get; set; } = 1;

        private double UpdateFrequency => DefaultUpdateFrequency / Speed; // [ms]

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

            graphics.Clear(Color.White);

            DrawField(graphics);

            if (Match != null)
            {
                var step = PlaySlider.Value;
                DrawBall(graphics, step);
                DrawPlayers(graphics, step);
            }

            graphics.Flush();
        }

        public void LoadMatch(Match match)
        {
            Match = match;
            GamePanel.Invalidate();
        }

        public void StartPlaying()
        {
            lock (this)
            {
                if (IsPlaying)
                    return;

                IsPlaying = true;
            }

            Task.Run(() => PlayMatch());
        }

        private void PlayMatch()
        {
            long lastUpdateTime = 0;

            while (IsPlaying)
            {
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

                    RenderCurrentState();
                }));
            }
        }

        public void RenderCurrentState()
        {
            UpdateStatus(PlaySlider.Value);
            GamePanel.Invalidate();
        }

        private void UpdateStatus(int step)
        {
            // time
            var totalSeconds = step * DefaultUpdateFrequency / 1000;
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds - minutes * 60;
            TimeLabel.Text = $"{minutes}:{seconds}";

            if (Match == null)
                return;

            // score
            Func<Team, int> getGoalsOfTeam = (team => (from goal in Match.MatchInfo.Goals
                                                       where goal.TeamThatScored == team
                                                       select goal.ScoreTime.Split(':') into tokens
                                                       let goalMinutes = int.Parse(tokens[0])
                                                       let goalSeconds = int.Parse(tokens[1])
                                                       where goalMinutes < minutes || goalMinutes == minutes && goalSeconds <= seconds
                                                       select goalMinutes).Count());

            var goals1 = getGoalsOfTeam(Team.FirstPlayer);
            var goals2 = getGoalsOfTeam(Team.SecondPlayer);

            ScoreLabel.Text = $"{goals1}:{goals2}";
        }

        private void DrawPlayers(Graphics graphics, int step)
        {
            var data = Match.MatchInfo.MatchData;

            // no more data to show, e.g. if the match was canceled
            if (data.Count < 46*step + 2 + 2*21 + 2)
                return;

            for (int i = 0; i < 11; i++)
            {
                // first team
                var x = data[46 * step + 2 + 2 * i];
                var y = data[46 * step + 2 + 2 * i + 1];
                DrawPlayer(graphics, x, y, Color.Red);

                // second team
                int j = i + 11;
                x = data[46 * step + 2 + 2 * j];
                y = data[46 * step + 2 + 2 * j + 1];
                DrawPlayer(graphics, x, y, Color.Blue);
            }

        }

        private void DrawPlayer(Graphics graphics, float x, float y, Color color)
        {
            var brush = new SolidBrush(color);

            graphics.FillEllipse(brush, x - 0.5f, y - 0.5f, 1, 1);
        }

        private void DrawBall(Graphics graphics, int step)
        {
            var data = Match.MatchInfo.MatchData;

            // no more data to show, e.g. if the match was canceled
            if (data.Count < 46*step + 2)
                return;

            var x = data[46 * step];
            var y = data[46 * step + 1];

            var brush = new SolidBrush(Color.Black);

            graphics.FillEllipse(brush, x - 0.22f, y - 0.22f, 2 * 0.22f, 2 * 0.22f);
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

            // central mark
            graphics.DrawEllipse(pen, 55 - 0.12f, 37.5f - 0.12f, 2 * 0.12f, 2 * 0.12f);

            // penalty marks
            graphics.DrawEllipse(pen, 11 - 0.12f, 37.5f - 0.12f, 2 * 0.12f, 2 * 0.12f);
            graphics.DrawEllipse(pen, 99 - 0.12f, 37.5f - 0.12f, 2 * 0.12f, 2 * 0.12f);

            // penalty arcs
            // ... acos(5.5 / 9.15) ~ 53.05 degrees
            graphics.DrawArc(pen, 11 - 9.15f, 37.5f - 9.15f, 2 * 9.15f, 2 * 9.15f, -53.05f, 2 * 53.05f);
            graphics.DrawArc(pen, 99 - 9.15f, 37.5f - 9.15f, 2 * 9.15f, 2 * 9.15f, 180 - 53.05f, 2 * 53.05f);

            // corner arcs
            graphics.DrawArc(pen, 0 - 1, 0 - 1, 2, 2, 0, 90);
            graphics.DrawArc(pen, 0 - 1, 75 - 1, 2, 2, 270, 90);
            graphics.DrawArc(pen, 110 - 1, 75 - 1, 2, 2, 180, 90);
            graphics.DrawArc(pen, 110 - 1, 0 - 1, 2, 2, 90, 90);

            // goal posts
            graphics.DrawRectangle(pen, -1.2f, 37.5f - 7.32f / 2, 1.2f, 7.32f);
            graphics.DrawRectangle(pen, 110, 37.5f - 7.32f / 2, 1.2f, 7.32f);

            // goal areas
            graphics.DrawRectangle(pen, 0, 37.5f - 9.16f, 5.5f, 2 * 9.16f);
            graphics.DrawRectangle(pen, 110 - 5.5f, 37.5f - 9.16f, 5.5f, 2 * 9.16f);

            // penalty areas
            graphics.DrawRectangle(pen, 0, 37.5f - 20.16f, 16.5f, 2 * 20.16f);
            graphics.DrawRectangle(pen, 110 - 16.5f, 37.5f - 20.16f, 16.5f, 2 * 20.16f);

        }

        public void StopPlaying()
        {
            IsPlaying = false;
        }

    }
}
