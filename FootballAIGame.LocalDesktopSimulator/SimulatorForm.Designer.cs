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
            this.SimulationPanel = new System.Windows.Forms.Panel();
            this.SimulationLabel = new System.Windows.Forms.Label();
            this.SimulationProgress = new System.Windows.Forms.ProgressBar();
            this.HeadingLabel = new System.Windows.Forms.Label();
            this.ErrorsListBox = new System.Windows.Forms.ListBox();
            this.GoalsListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.FinalShotsOnTargetLabel = new System.Windows.Forms.Label();
            this.FinalShotsLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.FinalScoreLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.LoadMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.RestartButton = new System.Windows.Forms.Button();
            this.SpeedDropDownList = new System.Windows.Forms.ComboBox();
            this.CurrentTimeLabel = new System.Windows.Forms.Label();
            this.CurrentTimeHeadingLabel = new System.Windows.Forms.Label();
            this.CurrentScoreHeadingLabel = new System.Windows.Forms.Label();
            this.CurrentScoreLabel = new System.Windows.Forms.Label();
            this.PlaySlider = new FootballAIGame.LocalDesktopSimulator.CustomControls.Slider();
            this.SimulationPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartMatchButton
            // 
            this.StartMatchButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.StartMatchButton.Location = new System.Drawing.Point(38, 12);
            this.StartMatchButton.Name = "StartMatchButton";
            this.StartMatchButton.Size = new System.Drawing.Size(138, 60);
            this.StartMatchButton.TabIndex = 0;
            this.StartMatchButton.Text = "Start match";
            this.StartMatchButton.UseVisualStyleBackColor = true;
            this.StartMatchButton.Click += new System.EventHandler(this.StartMatchButtonClick);
            // 
            // AiListBox
            // 
            this.AiListBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AiListBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.AiListBox.FormattingEnabled = true;
            this.AiListBox.HorizontalScrollbar = true;
            this.AiListBox.IntegralHeight = false;
            this.AiListBox.ItemHeight = 16;
            this.AiListBox.Location = new System.Drawing.Point(38, 106);
            this.AiListBox.Name = "AiListBox";
            this.AiListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.AiListBox.Size = new System.Drawing.Size(138, 413);
            this.AiListBox.TabIndex = 1;
            // 
            // SimulationPanel
            // 
            this.SimulationPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.SimulationPanel.Controls.Add(this.SimulationLabel);
            this.SimulationPanel.Controls.Add(this.SimulationProgress);
            this.SimulationPanel.Location = new System.Drawing.Point(279, 126);
            this.SimulationPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SimulationPanel.Name = "SimulationPanel";
            this.SimulationPanel.Size = new System.Drawing.Size(784, 555);
            this.SimulationPanel.TabIndex = 2;
            // 
            // SimulationLabel
            // 
            this.SimulationLabel.AutoSize = true;
            this.SimulationLabel.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SimulationLabel.Location = new System.Drawing.Point(322, 198);
            this.SimulationLabel.Name = "SimulationLabel";
            this.SimulationLabel.Size = new System.Drawing.Size(141, 29);
            this.SimulationLabel.TabIndex = 1;
            this.SimulationLabel.Text = "Simulating";
            this.SimulationLabel.Visible = false;
            // 
            // SimulationProgress
            // 
            this.SimulationProgress.Location = new System.Drawing.Point(0, 240);
            this.SimulationProgress.Name = "SimulationProgress";
            this.SimulationProgress.Size = new System.Drawing.Size(785, 36);
            this.SimulationProgress.TabIndex = 0;
            this.SimulationProgress.Visible = false;
            // 
            // HeadingLabel
            // 
            this.HeadingLabel.AutoSize = true;
            this.HeadingLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.HeadingLabel.Location = new System.Drawing.Point(602, 13);
            this.HeadingLabel.Name = "HeadingLabel";
            this.HeadingLabel.Size = new System.Drawing.Size(0, 21);
            this.HeadingLabel.TabIndex = 3;
            // 
            // ErrorsListBox
            // 
            this.ErrorsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.ErrorsListBox.FormattingEnabled = true;
            this.ErrorsListBox.HorizontalScrollbar = true;
            this.ErrorsListBox.ItemHeight = 16;
            this.ErrorsListBox.Location = new System.Drawing.Point(16, 371);
            this.ErrorsListBox.Name = "ErrorsListBox";
            this.ErrorsListBox.Size = new System.Drawing.Size(238, 196);
            this.ErrorsListBox.TabIndex = 4;
            // 
            // GoalsListBox
            // 
            this.GoalsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GoalsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.GoalsListBox.FormattingEnabled = true;
            this.GoalsListBox.HorizontalScrollbar = true;
            this.GoalsListBox.ItemHeight = 16;
            this.GoalsListBox.Location = new System.Drawing.Point(16, 172);
            this.GoalsListBox.Name = "GoalsListBox";
            this.GoalsListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.GoalsListBox.Size = new System.Drawing.Size(238, 164);
            this.GoalsListBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(112, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Goals";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(105, 351);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Errors";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(109, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Shots";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(77, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "Shots on target";
            // 
            // FinalShotsOnTargetLabel
            // 
            this.FinalShotsOnTargetLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FinalShotsOnTargetLabel.AutoSize = true;
            this.FinalShotsOnTargetLabel.Location = new System.Drawing.Point(116, 121);
            this.FinalShotsOnTargetLabel.Name = "FinalShotsOnTargetLabel";
            this.FinalShotsOnTargetLabel.Size = new System.Drawing.Size(36, 17);
            this.FinalShotsOnTargetLabel.TabIndex = 11;
            this.FinalShotsOnTargetLabel.Text = "0 / 0";
            // 
            // FinalShotsLabel
            // 
            this.FinalShotsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FinalShotsLabel.AutoSize = true;
            this.FinalShotsLabel.Location = new System.Drawing.Point(116, 87);
            this.FinalShotsLabel.Name = "FinalShotsLabel";
            this.FinalShotsLabel.Size = new System.Drawing.Size(36, 17);
            this.FinalShotsLabel.TabIndex = 12;
            this.FinalShotsLabel.Text = "0 / 0";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.FinalScoreLabel);
            this.panel1.Controls.Add(this.ErrorsListBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.FinalShotsOnTargetLabel);
            this.panel1.Controls.Add(this.GoalsListBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.FinalShotsLabel);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(2, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(271, 584);
            this.panel1.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label12.Location = new System.Drawing.Point(93, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87, 20);
            this.label12.TabIndex = 22;
            this.label12.Text = "Final Info";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label10.Location = new System.Drawing.Point(112, 35);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 17);
            this.label10.TabIndex = 13;
            this.label10.Text = "Score";
            // 
            // FinalScoreLabel
            // 
            this.FinalScoreLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FinalScoreLabel.AutoSize = true;
            this.FinalScoreLabel.Location = new System.Drawing.Point(117, 52);
            this.FinalScoreLabel.Name = "FinalScoreLabel";
            this.FinalScoreLabel.Size = new System.Drawing.Size(37, 17);
            this.FinalScoreLabel.TabIndex = 13;
            this.FinalScoreLabel.Text = "0 - 0";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.HeadingLabel);
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Location = new System.Drawing.Point(0, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1118, 34);
            this.panel2.TabIndex = 14;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadMatchToolStripMenuItem,
            this.SaveMatchToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1118, 28);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // LoadMatchToolStripMenuItem
            // 
            this.LoadMatchToolStripMenuItem.Name = "LoadMatchToolStripMenuItem";
            this.LoadMatchToolStripMenuItem.Size = new System.Drawing.Size(99, 24);
            this.LoadMatchToolStripMenuItem.Text = "Load Match";
            this.LoadMatchToolStripMenuItem.Click += new System.EventHandler(this.LoadMatchToolStripMenuItemClick);
            // 
            // SaveMatchToolStripMenuItem
            // 
            this.SaveMatchToolStripMenuItem.Name = "SaveMatchToolStripMenuItem";
            this.SaveMatchToolStripMenuItem.Size = new System.Drawing.Size(97, 24);
            this.SaveMatchToolStripMenuItem.Text = "Save Match";
            this.SaveMatchToolStripMenuItem.Click += new System.EventHandler(this.SaveMatchToolStripMenuItemClick);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.StartMatchButton);
            this.panel3.Controls.Add(this.AiListBox);
            this.panel3.Location = new System.Drawing.Point(1069, 126);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(215, 538);
            this.panel3.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label7.Location = new System.Drawing.Point(54, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Connected AI";
            // 
            // PlayButton
            // 
            this.PlayButton.Enabled = false;
            this.PlayButton.Location = new System.Drawing.Point(436, 53);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(151, 33);
            this.PlayButton.TabIndex = 15;
            this.PlayButton.Text = "Play / Pause";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // RestartButton
            // 
            this.RestartButton.Enabled = false;
            this.RestartButton.Location = new System.Drawing.Point(593, 53);
            this.RestartButton.Name = "RestartButton";
            this.RestartButton.Size = new System.Drawing.Size(151, 33);
            this.RestartButton.TabIndex = 16;
            this.RestartButton.Text = "Restart";
            this.RestartButton.UseVisualStyleBackColor = true;
            this.RestartButton.Click += new System.EventHandler(this.RestartButtonClick);
            // 
            // SpeedDropDownList
            // 
            this.SpeedDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SpeedDropDownList.FormattingEnabled = true;
            this.SpeedDropDownList.Items.AddRange(new object[] {
            "1x",
            "2x",
            "3x",
            "4x"});
            this.SpeedDropDownList.Location = new System.Drawing.Point(761, 59);
            this.SpeedDropDownList.Name = "SpeedDropDownList";
            this.SpeedDropDownList.Size = new System.Drawing.Size(87, 24);
            this.SpeedDropDownList.TabIndex = 17;
            this.SpeedDropDownList.SelectedIndexChanged += new System.EventHandler(this.SpeedDropDownListSelectedIndexChanged);
            // 
            // CurrentTimeLabel
            // 
            this.CurrentTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CurrentTimeLabel.AutoSize = true;
            this.CurrentTimeLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentTimeLabel.Location = new System.Drawing.Point(367, 62);
            this.CurrentTimeLabel.Name = "CurrentTimeLabel";
            this.CurrentTimeLabel.Size = new System.Drawing.Size(34, 21);
            this.CurrentTimeLabel.TabIndex = 14;
            this.CurrentTimeLabel.Text = "0:0";
            // 
            // CurrentTimeHeadingLabel
            // 
            this.CurrentTimeHeadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CurrentTimeHeadingLabel.AutoSize = true;
            this.CurrentTimeHeadingLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentTimeHeadingLabel.Location = new System.Drawing.Point(304, 62);
            this.CurrentTimeHeadingLabel.Name = "CurrentTimeHeadingLabel";
            this.CurrentTimeHeadingLabel.Size = new System.Drawing.Size(57, 21);
            this.CurrentTimeHeadingLabel.TabIndex = 21;
            this.CurrentTimeHeadingLabel.Text = "Time:";
            // 
            // CurrentScoreHeadingLabel
            // 
            this.CurrentScoreHeadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CurrentScoreHeadingLabel.AutoSize = true;
            this.CurrentScoreHeadingLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentScoreHeadingLabel.Location = new System.Drawing.Point(884, 62);
            this.CurrentScoreHeadingLabel.Name = "CurrentScoreHeadingLabel";
            this.CurrentScoreHeadingLabel.Size = new System.Drawing.Size(133, 21);
            this.CurrentScoreHeadingLabel.TabIndex = 22;
            this.CurrentScoreHeadingLabel.Text = "Current Score:";
            // 
            // CurrentScoreLabel
            // 
            this.CurrentScoreLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CurrentScoreLabel.AutoSize = true;
            this.CurrentScoreLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentScoreLabel.Location = new System.Drawing.Point(1023, 62);
            this.CurrentScoreLabel.Name = "CurrentScoreLabel";
            this.CurrentScoreLabel.Size = new System.Drawing.Size(34, 21);
            this.CurrentScoreLabel.TabIndex = 23;
            this.CurrentScoreLabel.Text = "0:0";
            // 
            // PlaySlider
            // 
            this.PlaySlider.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.PlaySlider.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.PlaySlider.Location = new System.Drawing.Point(279, 97);
            this.PlaySlider.Max = 1500;
            this.PlaySlider.Name = "PlaySlider";
            this.PlaySlider.Size = new System.Drawing.Size(784, 23);
            this.PlaySlider.TabIndex = 20;
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 693);
            this.Controls.Add(this.CurrentScoreLabel);
            this.Controls.Add(this.CurrentScoreHeadingLabel);
            this.Controls.Add(this.CurrentTimeHeadingLabel);
            this.Controls.Add(this.CurrentTimeLabel);
            this.Controls.Add(this.PlaySlider);
            this.Controls.Add(this.SpeedDropDownList);
            this.Controls.Add(this.RestartButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SimulationPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SimulatorForm";
            this.Text = "Football AI Game";
            this.SimulationPanel.ResumeLayout(false);
            this.SimulationPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartMatchButton;
        private System.Windows.Forms.ListBox AiListBox;
        private System.Windows.Forms.Panel SimulationPanel;
        private System.Windows.Forms.Label HeadingLabel;
        private System.Windows.Forms.ListBox ErrorsListBox;
        private System.Windows.Forms.ListBox GoalsListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label FinalShotsOnTargetLabel;
        private System.Windows.Forms.Label FinalShotsLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem LoadMatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveMatchToolStripMenuItem;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button RestartButton;
        private System.Windows.Forms.ComboBox SpeedDropDownList;
        private Slider PlaySlider;
        private Label label10;
        private Label FinalScoreLabel;
        private Label CurrentTimeLabel;
        private Label CurrentTimeHeadingLabel;
        private Label SimulationLabel;
        private ProgressBar SimulationProgress;
        private Label label12;
        private Label CurrentScoreHeadingLabel;
        private Label CurrentScoreLabel;
    }
}

