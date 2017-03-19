using System.Windows.Forms;
using FootballAIGame.LocalDesktopSimulator.CustomControls;

namespace FootballAIGame.LocalDesktopSimulator
{
    partial class SimulatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartMatchButton = new System.Windows.Forms.Button();
            this.AiListBox = new System.Windows.Forms.ListBox();
            this.ErrorsListBox = new System.Windows.Forms.ListBox();
            this.GoalsListBox = new System.Windows.Forms.ListBox();
            this.GoalsHeadingLabel = new System.Windows.Forms.Label();
            this.ErrorsHeadingLabel = new System.Windows.Forms.Label();
            this.ShotsHeadingLabel = new System.Windows.Forms.Label();
            this.ShotsOnTargetHeadingLabel = new System.Windows.Forms.Label();
            this.ShotsOnTargetLabel = new System.Windows.Forms.Label();
            this.ShotsLabel = new System.Windows.Forms.Label();
            this.FinalInfoHeadingLabel = new System.Windows.Forms.Label();
            this.ScoreHeadingLabel = new System.Windows.Forms.Label();
            this.ScoreLabel = new System.Windows.Forms.Label();
            this.ConnectedAiHeadingLabel = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.RestartButton = new System.Windows.Forms.Button();
            this.SpeedDropDownList = new System.Windows.Forms.ComboBox();
            this.CurrentTimeLabel = new System.Windows.Forms.Label();
            this.CurrentTimeHeadingLabel = new System.Windows.Forms.Label();
            this.CurrentScoreHeadingLabel = new System.Windows.Forms.Label();
            this.CurrentScoreLabel = new System.Windows.Forms.Label();
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.MatchPanel = new FootballAIGame.LocalDesktopSimulator.CustomControls.GamePanel();
            this.SimulationLabel = new System.Windows.Forms.Label();
            this.SimulationProgress = new System.Windows.Forms.ProgressBar();
            this.PlaySlider = new FootballAIGame.LocalDesktopSimulator.CustomControls.Slider();
            this.MatchDetailsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.StartNewMatchTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.MenuPanel = new System.Windows.Forms.Panel();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.LoadMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TopTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SecondAiLabel = new System.Windows.Forms.Label();
            this.VsLabel = new System.Windows.Forms.Label();
            this.FirstAiLabel = new System.Windows.Forms.Label();
            this.WatchMatchTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.MainTableLayoutPanel.SuspendLayout();
            this.MatchPanel.SuspendLayout();
            this.MatchDetailsTableLayoutPanel.SuspendLayout();
            this.StartNewMatchTableLayoutPanel.SuspendLayout();
            this.MenuPanel.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.TopTableLayoutPanel.SuspendLayout();
            this.WatchMatchTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartMatchButton
            // 
            this.StartMatchButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.StartMatchButton.Location = new System.Drawing.Point(7, 8);
            this.StartMatchButton.Name = "StartMatchButton";
            this.StartMatchButton.Size = new System.Drawing.Size(169, 37);
            this.StartMatchButton.TabIndex = 0;
            this.StartMatchButton.Text = "Start match";
            this.StartMatchButton.UseVisualStyleBackColor = true;
            this.StartMatchButton.Click += new System.EventHandler(this.StartMatchButtonClick);
            // 
            // AiListBox
            // 
            this.AiListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.AiListBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.AiListBox.FormattingEnabled = true;
            this.AiListBox.HorizontalScrollbar = true;
            this.AiListBox.IntegralHeight = false;
            this.AiListBox.ItemHeight = 16;
            this.AiListBox.Location = new System.Drawing.Point(12, 84);
            this.AiListBox.Name = "AiListBox";
            this.AiListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.AiListBox.Size = new System.Drawing.Size(160, 458);
            this.AiListBox.TabIndex = 1;
            // 
            // ErrorsListBox
            // 
            this.ErrorsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.ErrorsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.ErrorsListBox.FormattingEnabled = true;
            this.ErrorsListBox.HorizontalScrollbar = true;
            this.ErrorsListBox.ItemHeight = 16;
            this.ErrorsListBox.Location = new System.Drawing.Point(30, 379);
            this.ErrorsListBox.Name = "ErrorsListBox";
            this.ErrorsListBox.Size = new System.Drawing.Size(186, 148);
            this.ErrorsListBox.TabIndex = 4;
            // 
            // GoalsListBox
            // 
            this.GoalsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.GoalsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.GoalsListBox.FormattingEnabled = true;
            this.GoalsListBox.HorizontalScrollbar = true;
            this.GoalsListBox.ItemHeight = 16;
            this.GoalsListBox.Location = new System.Drawing.Point(30, 208);
            this.GoalsListBox.Name = "GoalsListBox";
            this.GoalsListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.GoalsListBox.Size = new System.Drawing.Size(186, 132);
            this.GoalsListBox.TabIndex = 5;
            // 
            // GoalsHeadingLabel
            // 
            this.GoalsHeadingLabel.AutoSize = true;
            this.GoalsHeadingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GoalsHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GoalsHeadingLabel.Location = new System.Drawing.Point(3, 181);
            this.GoalsHeadingLabel.Name = "GoalsHeadingLabel";
            this.GoalsHeadingLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.GoalsHeadingLabel.Size = new System.Drawing.Size(240, 24);
            this.GoalsHeadingLabel.TabIndex = 6;
            this.GoalsHeadingLabel.Text = "Goals";
            this.GoalsHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ErrorsHeadingLabel
            // 
            this.ErrorsHeadingLabel.AutoSize = true;
            this.ErrorsHeadingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorsHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ErrorsHeadingLabel.Location = new System.Drawing.Point(3, 347);
            this.ErrorsHeadingLabel.Name = "ErrorsHeadingLabel";
            this.ErrorsHeadingLabel.Size = new System.Drawing.Size(240, 29);
            this.ErrorsHeadingLabel.TabIndex = 7;
            this.ErrorsHeadingLabel.Text = "Errors";
            this.ErrorsHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShotsHeadingLabel
            // 
            this.ShotsHeadingLabel.AutoSize = true;
            this.ShotsHeadingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShotsHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ShotsHeadingLabel.Location = new System.Drawing.Point(3, 76);
            this.ShotsHeadingLabel.Name = "ShotsHeadingLabel";
            this.ShotsHeadingLabel.Size = new System.Drawing.Size(240, 27);
            this.ShotsHeadingLabel.TabIndex = 9;
            this.ShotsHeadingLabel.Text = "Shots";
            this.ShotsHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShotsOnTargetHeadingLabel
            // 
            this.ShotsOnTargetHeadingLabel.AutoSize = true;
            this.ShotsOnTargetHeadingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShotsOnTargetHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ShotsOnTargetHeadingLabel.Location = new System.Drawing.Point(3, 128);
            this.ShotsOnTargetHeadingLabel.Name = "ShotsOnTargetHeadingLabel";
            this.ShotsOnTargetHeadingLabel.Size = new System.Drawing.Size(240, 25);
            this.ShotsOnTargetHeadingLabel.TabIndex = 10;
            this.ShotsOnTargetHeadingLabel.Text = "Shots on target";
            this.ShotsOnTargetHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShotsOnTargetLabel
            // 
            this.ShotsOnTargetLabel.AutoSize = true;
            this.ShotsOnTargetLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShotsOnTargetLabel.Location = new System.Drawing.Point(3, 153);
            this.ShotsOnTargetLabel.Name = "ShotsOnTargetLabel";
            this.ShotsOnTargetLabel.Size = new System.Drawing.Size(240, 28);
            this.ShotsOnTargetLabel.TabIndex = 11;
            this.ShotsOnTargetLabel.Text = "0 / 0";
            this.ShotsOnTargetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShotsLabel
            // 
            this.ShotsLabel.AutoSize = true;
            this.ShotsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShotsLabel.Location = new System.Drawing.Point(3, 103);
            this.ShotsLabel.Name = "ShotsLabel";
            this.ShotsLabel.Size = new System.Drawing.Size(240, 25);
            this.ShotsLabel.TabIndex = 12;
            this.ShotsLabel.Text = "0 / 0";
            this.ShotsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FinalInfoHeadingLabel
            // 
            this.FinalInfoHeadingLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.FinalInfoHeadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FinalInfoHeadingLabel.Location = new System.Drawing.Point(3, 0);
            this.FinalInfoHeadingLabel.Name = "FinalInfoHeadingLabel";
            this.FinalInfoHeadingLabel.Size = new System.Drawing.Size(240, 22);
            this.FinalInfoHeadingLabel.TabIndex = 22;
            this.FinalInfoHeadingLabel.Text = "Final Info";
            this.FinalInfoHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScoreHeadingLabel
            // 
            this.ScoreHeadingLabel.AutoSize = true;
            this.ScoreHeadingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScoreHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ScoreHeadingLabel.Location = new System.Drawing.Point(3, 24);
            this.ScoreHeadingLabel.Name = "ScoreHeadingLabel";
            this.ScoreHeadingLabel.Size = new System.Drawing.Size(240, 26);
            this.ScoreHeadingLabel.TabIndex = 13;
            this.ScoreHeadingLabel.Text = "Score";
            this.ScoreHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScoreLabel
            // 
            this.ScoreLabel.AutoSize = true;
            this.ScoreLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScoreLabel.Location = new System.Drawing.Point(3, 50);
            this.ScoreLabel.Name = "ScoreLabel";
            this.ScoreLabel.Size = new System.Drawing.Size(240, 26);
            this.ScoreLabel.TabIndex = 13;
            this.ScoreLabel.Text = "0 - 0";
            this.ScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ConnectedAiHeadingLabel
            // 
            this.ConnectedAiHeadingLabel.AutoSize = true;
            this.ConnectedAiHeadingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConnectedAiHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ConnectedAiHeadingLabel.Location = new System.Drawing.Point(3, 54);
            this.ConnectedAiHeadingLabel.Name = "ConnectedAiHeadingLabel";
            this.ConnectedAiHeadingLabel.Size = new System.Drawing.Size(178, 27);
            this.ConnectedAiHeadingLabel.TabIndex = 2;
            this.ConnectedAiHeadingLabel.Text = "Connected AI";
            this.ConnectedAiHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayButton
            // 
            this.PlayButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayButton.Enabled = false;
            this.PlayButton.Location = new System.Drawing.Point(221, 3);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(181, 35);
            this.PlayButton.TabIndex = 15;
            this.PlayButton.Text = "Play / Pause";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // RestartButton
            // 
            this.RestartButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RestartButton.Enabled = false;
            this.RestartButton.Location = new System.Drawing.Point(408, 3);
            this.RestartButton.Name = "RestartButton";
            this.RestartButton.Size = new System.Drawing.Size(124, 35);
            this.RestartButton.TabIndex = 16;
            this.RestartButton.Text = "Restart";
            this.RestartButton.UseVisualStyleBackColor = true;
            this.RestartButton.Click += new System.EventHandler(this.RestartButtonClick);
            // 
            // SpeedDropDownList
            // 
            this.SpeedDropDownList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SpeedDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SpeedDropDownList.FormattingEnabled = true;
            this.SpeedDropDownList.Items.AddRange(new object[] {
            "1x",
            "2x",
            "3x",
            "4x"});
            this.SpeedDropDownList.Location = new System.Drawing.Point(546, 8);
            this.SpeedDropDownList.Margin = new System.Windows.Forms.Padding(0);
            this.SpeedDropDownList.Name = "SpeedDropDownList";
            this.SpeedDropDownList.Size = new System.Drawing.Size(51, 24);
            this.SpeedDropDownList.TabIndex = 17;
            this.SpeedDropDownList.SelectedIndexChanged += new System.EventHandler(this.SpeedDropDownListSelectedIndexChanged);
            // 
            // CurrentTimeLabel
            // 
            this.CurrentTimeLabel.AutoSize = true;
            this.CurrentTimeLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.CurrentTimeLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentTimeLabel.Location = new System.Drawing.Point(116, 0);
            this.CurrentTimeLabel.Name = "CurrentTimeLabel";
            this.CurrentTimeLabel.Size = new System.Drawing.Size(34, 41);
            this.CurrentTimeLabel.TabIndex = 14;
            this.CurrentTimeLabel.Text = "0:0";
            this.CurrentTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CurrentTimeHeadingLabel
            // 
            this.CurrentTimeHeadingLabel.AutoSize = true;
            this.CurrentTimeHeadingLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.CurrentTimeHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentTimeHeadingLabel.Location = new System.Drawing.Point(64, 0);
            this.CurrentTimeHeadingLabel.Name = "CurrentTimeHeadingLabel";
            this.CurrentTimeHeadingLabel.Size = new System.Drawing.Size(46, 41);
            this.CurrentTimeHeadingLabel.TabIndex = 21;
            this.CurrentTimeHeadingLabel.Text = "Time:";
            this.CurrentTimeHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CurrentScoreHeadingLabel
            // 
            this.CurrentScoreHeadingLabel.AutoSize = true;
            this.CurrentScoreHeadingLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.CurrentScoreHeadingLabel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentScoreHeadingLabel.Location = new System.Drawing.Point(658, 0);
            this.CurrentScoreHeadingLabel.Name = "CurrentScoreHeadingLabel";
            this.CurrentScoreHeadingLabel.Size = new System.Drawing.Size(52, 41);
            this.CurrentScoreHeadingLabel.TabIndex = 22;
            this.CurrentScoreHeadingLabel.Text = "Score:";
            this.CurrentScoreHeadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CurrentScoreLabel
            // 
            this.CurrentScoreLabel.AutoSize = true;
            this.CurrentScoreLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.CurrentScoreLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentScoreLabel.Location = new System.Drawing.Point(716, 0);
            this.CurrentScoreLabel.Name = "CurrentScoreLabel";
            this.CurrentScoreLabel.Size = new System.Drawing.Size(34, 41);
            this.CurrentScoreLabel.TabIndex = 23;
            this.CurrentScoreLabel.Text = "0:0";
            this.CurrentScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.ColumnCount = 3;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.MainTableLayoutPanel.Controls.Add(this.MatchPanel, 1, 3);
            this.MainTableLayoutPanel.Controls.Add(this.PlaySlider, 1, 2);
            this.MainTableLayoutPanel.Controls.Add(this.MatchDetailsTableLayoutPanel, 0, 3);
            this.MainTableLayoutPanel.Controls.Add(this.StartNewMatchTableLayoutPanel, 2, 3);
            this.MainTableLayoutPanel.Controls.Add(this.MenuPanel, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.WatchMatchTableLayoutPanel, 1, 1);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 5;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82F));
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(1262, 673);
            this.MainTableLayoutPanel.TabIndex = 24;
            // 
            // MatchPanel
            // 
            this.MatchPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MatchPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.MatchPanel.Controls.Add(this.SimulationLabel);
            this.MatchPanel.Controls.Add(this.SimulationProgress);
            this.MatchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MatchPanel.Location = new System.Drawing.Point(252, 113);
            this.MatchPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MatchPanel.Name = "MatchPanel";
            this.MatchPanel.Size = new System.Drawing.Size(820, 551);
            this.MatchPanel.TabIndex = 2;
            // 
            // SimulationLabel
            // 
            this.SimulationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SimulationLabel.AutoSize = true;
            this.SimulationLabel.BackColor = System.Drawing.Color.Transparent;
            this.SimulationLabel.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SimulationLabel.Location = new System.Drawing.Point(345, 178);
            this.SimulationLabel.Name = "SimulationLabel";
            this.SimulationLabel.Size = new System.Drawing.Size(141, 29);
            this.SimulationLabel.TabIndex = 1;
            this.SimulationLabel.Text = "Simulating";
            this.SimulationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SimulationLabel.Visible = false;
            // 
            // SimulationProgress
            // 
            this.SimulationProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SimulationProgress.Location = new System.Drawing.Point(0, 242);
            this.SimulationProgress.Maximum = 1500;
            this.SimulationProgress.Name = "SimulationProgress";
            this.SimulationProgress.Size = new System.Drawing.Size(820, 64);
            this.SimulationProgress.TabIndex = 0;
            this.SimulationProgress.Visible = false;
            // 
            // PlaySlider
            // 
            this.PlaySlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlaySlider.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.PlaySlider.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PlaySlider.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.PlaySlider.Location = new System.Drawing.Point(252, 81);
            this.PlaySlider.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.PlaySlider.Max = 1500;
            this.PlaySlider.Name = "PlaySlider";
            this.PlaySlider.Size = new System.Drawing.Size(820, 31);
            this.PlaySlider.TabIndex = 20;
            // 
            // MatchDetailsTableLayoutPanel
            // 
            this.MatchDetailsTableLayoutPanel.ColumnCount = 1;
            this.MatchDetailsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ErrorsListBox, 0, 10);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ShotsOnTargetLabel, 0, 6);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ErrorsHeadingLabel, 0, 9);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.FinalInfoHeadingLabel, 0, 0);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.GoalsListBox, 0, 8);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ShotsOnTargetHeadingLabel, 0, 5);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.GoalsHeadingLabel, 0, 7);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ShotsHeadingLabel, 0, 3);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ShotsLabel, 0, 4);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ScoreHeadingLabel, 0, 1);
            this.MatchDetailsTableLayoutPanel.Controls.Add(this.ScoreLabel, 0, 2);
            this.MatchDetailsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MatchDetailsTableLayoutPanel.Location = new System.Drawing.Point(3, 116);
            this.MatchDetailsTableLayoutPanel.Name = "MatchDetailsTableLayoutPanel";
            this.MatchDetailsTableLayoutPanel.RowCount = 11;
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.48505F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.817276F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.817276F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.149502F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.651163F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.651163F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.315615F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.48505F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.24585F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.481728F));
            this.MatchDetailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.39867F));
            this.MatchDetailsTableLayoutPanel.Size = new System.Drawing.Size(246, 545);
            this.MatchDetailsTableLayoutPanel.TabIndex = 16;
            // 
            // StartNewMatchTableLayoutPanel
            // 
            this.StartNewMatchTableLayoutPanel.ColumnCount = 1;
            this.StartNewMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.StartNewMatchTableLayoutPanel.Controls.Add(this.AiListBox, 0, 2);
            this.StartNewMatchTableLayoutPanel.Controls.Add(this.ConnectedAiHeadingLabel, 0, 1);
            this.StartNewMatchTableLayoutPanel.Controls.Add(this.StartMatchButton, 0, 0);
            this.StartNewMatchTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StartNewMatchTableLayoutPanel.Location = new System.Drawing.Point(1075, 116);
            this.StartNewMatchTableLayoutPanel.Name = "StartNewMatchTableLayoutPanel";
            this.StartNewMatchTableLayoutPanel.RowCount = 3;
            this.StartNewMatchTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.StartNewMatchTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.StartNewMatchTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.StartNewMatchTableLayoutPanel.Size = new System.Drawing.Size(184, 545);
            this.StartNewMatchTableLayoutPanel.TabIndex = 17;
            // 
            // MenuPanel
            // 
            this.MainTableLayoutPanel.SetColumnSpan(this.MenuPanel, 3);
            this.MenuPanel.Controls.Add(this.MainMenu);
            this.MenuPanel.Controls.Add(this.TopTableLayoutPanel);
            this.MenuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenuPanel.Location = new System.Drawing.Point(3, 3);
            this.MenuPanel.Name = "MenuPanel";
            this.MenuPanel.Size = new System.Drawing.Size(1256, 27);
            this.MenuPanel.TabIndex = 14;
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.Control;
            this.MainMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.MainMenu.GripMargin = new System.Windows.Forms.Padding(0);
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadMatchToolStripMenuItem,
            this.SaveMatchToolStripMenuItem});
            this.MainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Padding = new System.Windows.Forms.Padding(0);
            this.MainMenu.Size = new System.Drawing.Size(198, 27);
            this.MainMenu.TabIndex = 4;
            this.MainMenu.Text = "menuStrip1";
            // 
            // LoadMatchToolStripMenuItem
            // 
            this.LoadMatchToolStripMenuItem.Name = "LoadMatchToolStripMenuItem";
            this.LoadMatchToolStripMenuItem.Size = new System.Drawing.Size(99, 27);
            this.LoadMatchToolStripMenuItem.Text = "Load Match";
            this.LoadMatchToolStripMenuItem.Click += new System.EventHandler(this.LoadMatchToolStripMenuItemClick);
            // 
            // SaveMatchToolStripMenuItem
            // 
            this.SaveMatchToolStripMenuItem.Name = "SaveMatchToolStripMenuItem";
            this.SaveMatchToolStripMenuItem.Size = new System.Drawing.Size(97, 27);
            this.SaveMatchToolStripMenuItem.Text = "Save Match";
            this.SaveMatchToolStripMenuItem.Click += new System.EventHandler(this.SaveMatchToolStripMenuItemClick);
            // 
            // TopTableLayoutPanel
            // 
            this.TopTableLayoutPanel.ColumnCount = 5;
            this.TopTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TopTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.5F));
            this.TopTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.TopTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.5F));
            this.TopTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.TopTableLayoutPanel.Controls.Add(this.SecondAiLabel, 3, 0);
            this.TopTableLayoutPanel.Controls.Add(this.VsLabel, 2, 0);
            this.TopTableLayoutPanel.Controls.Add(this.FirstAiLabel, 1, 0);
            this.TopTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TopTableLayoutPanel.Name = "TopTableLayoutPanel";
            this.TopTableLayoutPanel.RowCount = 1;
            this.TopTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TopTableLayoutPanel.Size = new System.Drawing.Size(1256, 27);
            this.TopTableLayoutPanel.TabIndex = 2;
            // 
            // SecondAiLabel
            // 
            this.SecondAiLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SecondAiLabel.AutoSize = true;
            this.SecondAiLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SecondAiLabel.Location = new System.Drawing.Point(684, 3);
            this.SecondAiLabel.Margin = new System.Windows.Forms.Padding(0);
            this.SecondAiLabel.Name = "SecondAiLabel";
            this.SecondAiLabel.Size = new System.Drawing.Size(93, 21);
            this.SecondAiLabel.TabIndex = 5;
            this.SecondAiLabel.Text = "SecondAI";
            this.SecondAiLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SecondAiLabel.Visible = false;
            // 
            // VsLabel
            // 
            this.VsLabel.AutoSize = true;
            this.VsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VsLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.VsLabel.Location = new System.Drawing.Point(634, 0);
            this.VsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.VsLabel.Name = "VsLabel";
            this.VsLabel.Size = new System.Drawing.Size(50, 27);
            this.VsLabel.TabIndex = 3;
            this.VsLabel.Text = "vs";
            this.VsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.VsLabel.Visible = false;
            // 
            // FirstAiLabel
            // 
            this.FirstAiLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FirstAiLabel.AutoSize = true;
            this.FirstAiLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FirstAiLabel.Location = new System.Drawing.Point(566, 3);
            this.FirstAiLabel.Margin = new System.Windows.Forms.Padding(0);
            this.FirstAiLabel.Name = "FirstAiLabel";
            this.FirstAiLabel.Size = new System.Drawing.Size(68, 21);
            this.FirstAiLabel.TabIndex = 4;
            this.FirstAiLabel.Text = "FirstAI";
            this.FirstAiLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.FirstAiLabel.Visible = false;
            // 
            // WatchMatchTableLayoutPanel
            // 
            this.WatchMatchTableLayoutPanel.ColumnCount = 7;
            this.WatchMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.WatchMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.WatchMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23F));
            this.WatchMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.WatchMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.WatchMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.WatchMatchTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.WatchMatchTableLayoutPanel.Controls.Add(this.CurrentScoreLabel, 6, 0);
            this.WatchMatchTableLayoutPanel.Controls.Add(this.CurrentTimeLabel, 1, 0);
            this.WatchMatchTableLayoutPanel.Controls.Add(this.CurrentScoreHeadingLabel, 5, 0);
            this.WatchMatchTableLayoutPanel.Controls.Add(this.PlayButton, 2, 0);
            this.WatchMatchTableLayoutPanel.Controls.Add(this.RestartButton, 3, 0);
            this.WatchMatchTableLayoutPanel.Controls.Add(this.CurrentTimeHeadingLabel, 0, 0);
            this.WatchMatchTableLayoutPanel.Controls.Add(this.SpeedDropDownList, 4, 0);
            this.WatchMatchTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WatchMatchTableLayoutPanel.Location = new System.Drawing.Point(255, 36);
            this.WatchMatchTableLayoutPanel.Name = "WatchMatchTableLayoutPanel";
            this.WatchMatchTableLayoutPanel.RowCount = 1;
            this.WatchMatchTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.WatchMatchTableLayoutPanel.Size = new System.Drawing.Size(814, 41);
            this.WatchMatchTableLayoutPanel.TabIndex = 15;
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.MainTableLayoutPanel);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.MainMenu;
            this.MinimumSize = new System.Drawing.Size(960, 540);
            this.Name = "SimulatorForm";
            this.Text = "Football AI Game";
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.MatchPanel.ResumeLayout(false);
            this.MatchPanel.PerformLayout();
            this.MatchDetailsTableLayoutPanel.ResumeLayout(false);
            this.MatchDetailsTableLayoutPanel.PerformLayout();
            this.StartNewMatchTableLayoutPanel.ResumeLayout(false);
            this.StartNewMatchTableLayoutPanel.PerformLayout();
            this.MenuPanel.ResumeLayout(false);
            this.MenuPanel.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.TopTableLayoutPanel.ResumeLayout(false);
            this.TopTableLayoutPanel.PerformLayout();
            this.WatchMatchTableLayoutPanel.ResumeLayout(false);
            this.WatchMatchTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartMatchButton;
        private System.Windows.Forms.ListBox AiListBox;
        private System.Windows.Forms.ListBox ErrorsListBox;
        private System.Windows.Forms.ListBox GoalsListBox;
        private System.Windows.Forms.Label GoalsHeadingLabel;
        private System.Windows.Forms.Label ErrorsHeadingLabel;
        private System.Windows.Forms.Label ShotsHeadingLabel;
        private System.Windows.Forms.Label ShotsOnTargetHeadingLabel;
        private System.Windows.Forms.Label ShotsOnTargetLabel;
        private System.Windows.Forms.Label ShotsLabel;
        private System.Windows.Forms.Label ConnectedAiHeadingLabel;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button RestartButton;
        private System.Windows.Forms.ComboBox SpeedDropDownList;
        private Slider PlaySlider;
        private Label ScoreHeadingLabel;
        private Label ScoreLabel;
        private Label CurrentTimeLabel;
        private Label CurrentTimeHeadingLabel;
        private Label SimulationLabel;
        private ProgressBar SimulationProgress;
        private Label FinalInfoHeadingLabel;
        private Label CurrentScoreHeadingLabel;
        private Label CurrentScoreLabel;
        private GamePanel MatchPanel;
        private TableLayoutPanel MainTableLayoutPanel;
        private TableLayoutPanel WatchMatchTableLayoutPanel;
        private TableLayoutPanel MatchDetailsTableLayoutPanel;
        private TableLayoutPanel StartNewMatchTableLayoutPanel;
        private MenuStrip MainMenu;
        private ToolStripMenuItem LoadMatchToolStripMenuItem;
        private ToolStripMenuItem SaveMatchToolStripMenuItem;
        private Panel MenuPanel;
        private Label VsLabel;
        private TableLayoutPanel TopTableLayoutPanel;
        private Label SecondAiLabel;
        private Label FirstAiLabel;
    }
}

