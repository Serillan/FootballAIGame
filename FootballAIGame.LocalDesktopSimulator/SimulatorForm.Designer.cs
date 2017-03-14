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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.LoadMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label7 = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.RestartButton = new System.Windows.Forms.Button();
            this.SpeedDropDownList = new System.Windows.Forms.ComboBox();
            this.CurrentTimeLabel = new System.Windows.Forms.Label();
            this.CurrentTimeHeadingLabel = new System.Windows.Forms.Label();
            this.CurrentScoreHeadingLabel = new System.Windows.Forms.Label();
            this.CurrentScoreLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.HeadingLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.SimulationPanel = new FootballAIGame.LocalDesktopSimulator.CustomControls.DoubleBufferedPanel();
            this.SimulationLabel = new System.Windows.Forms.Label();
            this.SimulationProgress = new System.Windows.Forms.ProgressBar();
            this.PlaySlider = new FootballAIGame.LocalDesktopSimulator.CustomControls.Slider();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SimulationPanel.SuspendLayout();
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
            this.AiListBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.AiListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AiListBox.FormattingEnabled = true;
            this.AiListBox.HorizontalScrollbar = true;
            this.AiListBox.IntegralHeight = false;
            this.AiListBox.ItemHeight = 16;
            this.AiListBox.Location = new System.Drawing.Point(3, 84);
            this.AiListBox.Name = "AiListBox";
            this.AiListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.AiListBox.Size = new System.Drawing.Size(178, 458);
            this.AiListBox.TabIndex = 1;
            // 
            // ErrorsListBox
            // 
            this.ErrorsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.ErrorsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorsListBox.FormattingEnabled = true;
            this.ErrorsListBox.HorizontalScrollbar = true;
            this.ErrorsListBox.ItemHeight = 16;
            this.ErrorsListBox.Location = new System.Drawing.Point(3, 379);
            this.ErrorsListBox.Name = "ErrorsListBox";
            this.ErrorsListBox.Size = new System.Drawing.Size(240, 163);
            this.ErrorsListBox.TabIndex = 4;
            // 
            // GoalsListBox
            // 
            this.GoalsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.GoalsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GoalsListBox.FormattingEnabled = true;
            this.GoalsListBox.HorizontalScrollbar = true;
            this.GoalsListBox.ItemHeight = 16;
            this.GoalsListBox.Location = new System.Drawing.Point(3, 208);
            this.GoalsListBox.Name = "GoalsListBox";
            this.GoalsListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.GoalsListBox.Size = new System.Drawing.Size(240, 136);
            this.GoalsListBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(3, 181);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(240, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Goals";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(3, 347);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(240, 29);
            this.label2.TabIndex = 7;
            this.label2.Text = "Errors";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(3, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 27);
            this.label4.TabIndex = 9;
            this.label4.Text = "Shots";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(3, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(240, 25);
            this.label5.TabIndex = 10;
            this.label5.Text = "Shots on target";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FinalShotsOnTargetLabel
            // 
            this.FinalShotsOnTargetLabel.AutoSize = true;
            this.FinalShotsOnTargetLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FinalShotsOnTargetLabel.Location = new System.Drawing.Point(3, 153);
            this.FinalShotsOnTargetLabel.Name = "FinalShotsOnTargetLabel";
            this.FinalShotsOnTargetLabel.Size = new System.Drawing.Size(240, 28);
            this.FinalShotsOnTargetLabel.TabIndex = 11;
            this.FinalShotsOnTargetLabel.Text = "0 / 0";
            this.FinalShotsOnTargetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FinalShotsLabel
            // 
            this.FinalShotsLabel.AutoSize = true;
            this.FinalShotsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FinalShotsLabel.Location = new System.Drawing.Point(3, 103);
            this.FinalShotsLabel.Name = "FinalShotsLabel";
            this.FinalShotsLabel.Size = new System.Drawing.Size(240, 25);
            this.FinalShotsLabel.TabIndex = 12;
            this.FinalShotsLabel.Text = "0 / 0";
            this.FinalShotsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Location = new System.Drawing.Point(13, 114);
            this.panel1.MaximumSize = new System.Drawing.Size(248, 2000);
            this.panel1.MinimumSize = new System.Drawing.Size(140, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(140, 0);
            this.panel1.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Top;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label12.Location = new System.Drawing.Point(3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(240, 22);
            this.label12.TabIndex = 22;
            this.label12.Text = "Final Info";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label10.Location = new System.Drawing.Point(3, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(240, 26);
            this.label10.TabIndex = 13;
            this.label10.Text = "Score";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FinalScoreLabel
            // 
            this.FinalScoreLabel.AutoSize = true;
            this.FinalScoreLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FinalScoreLabel.Location = new System.Drawing.Point(3, 50);
            this.FinalScoreLabel.Name = "FinalScoreLabel";
            this.FinalScoreLabel.Size = new System.Drawing.Size(240, 26);
            this.FinalScoreLabel.TabIndex = 13;
            this.FinalScoreLabel.Text = "0 - 0";
            this.FinalScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadMatchToolStripMenuItem,
            this.SaveMatchToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Size = new System.Drawing.Size(1181, 22);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // LoadMatchToolStripMenuItem
            // 
            this.LoadMatchToolStripMenuItem.Name = "LoadMatchToolStripMenuItem";
            this.LoadMatchToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.LoadMatchToolStripMenuItem.Text = "Load Match";
            this.LoadMatchToolStripMenuItem.Click += new System.EventHandler(this.LoadMatchToolStripMenuItemClick);
            // 
            // SaveMatchToolStripMenuItem
            // 
            this.SaveMatchToolStripMenuItem.Name = "SaveMatchToolStripMenuItem";
            this.SaveMatchToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.SaveMatchToolStripMenuItem.Text = "Save Match";
            this.SaveMatchToolStripMenuItem.Click += new System.EventHandler(this.SaveMatchToolStripMenuItemClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label7.Location = new System.Drawing.Point(3, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(178, 27);
            this.label7.TabIndex = 2;
            this.label7.Text = "Connected AI";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayButton
            // 
            this.PlayButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayButton.Enabled = false;
            this.PlayButton.Location = new System.Drawing.Point(203, 3);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(163, 35);
            this.PlayButton.TabIndex = 15;
            this.PlayButton.Text = "Play / Pause";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // RestartButton
            // 
            this.RestartButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RestartButton.Enabled = false;
            this.RestartButton.Location = new System.Drawing.Point(372, 3);
            this.RestartButton.Name = "RestartButton";
            this.RestartButton.Size = new System.Drawing.Size(131, 35);
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
            this.SpeedDropDownList.Location = new System.Drawing.Point(511, 8);
            this.SpeedDropDownList.Name = "SpeedDropDownList";
            this.SpeedDropDownList.Size = new System.Drawing.Size(52, 24);
            this.SpeedDropDownList.TabIndex = 17;
            this.SpeedDropDownList.SelectedIndexChanged += new System.EventHandler(this.SpeedDropDownListSelectedIndexChanged);
            // 
            // CurrentTimeLabel
            // 
            this.CurrentTimeLabel.AutoSize = true;
            this.CurrentTimeLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.CurrentTimeLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CurrentTimeLabel.Location = new System.Drawing.Point(111, 0);
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
            this.CurrentTimeHeadingLabel.Location = new System.Drawing.Point(59, 0);
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
            this.CurrentScoreHeadingLabel.Location = new System.Drawing.Point(644, 0);
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
            this.CurrentScoreLabel.Location = new System.Drawing.Point(702, 0);
            this.CurrentScoreLabel.Name = "CurrentScoreLabel";
            this.CurrentScoreLabel.Size = new System.Drawing.Size(34, 41);
            this.CurrentScoreLabel.TabIndex = 23;
            this.CurrentScoreLabel.Text = "0:0";
            this.CurrentScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.Controls.Add(this.SimulationPanel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.PlaySlider, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1262, 673);
            this.tableLayoutPanel1.TabIndex = 24;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.ErrorsListBox, 0, 10);
            this.tableLayoutPanel3.Controls.Add(this.FinalShotsOnTargetLabel, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 9);
            this.tableLayoutPanel3.Controls.Add(this.label12, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.GoalsListBox, 0, 8);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.FinalShotsLabel, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label10, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.FinalScoreLabel, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 116);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 11;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.48505F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.817276F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.817276F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.149502F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.651163F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.651163F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.315615F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.48505F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.24585F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.481728F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.39867F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(246, 545);
            this.tableLayoutPanel3.TabIndex = 16;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.AiListBox, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.StartMatchButton, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(1075, 116);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(184, 545);
            this.tableLayoutPanel4.TabIndex = 17;
            // 
            // panel2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel2, 3);
            this.panel2.Controls.Add(this.HeadingLabel);
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1181, 22);
            this.panel2.TabIndex = 14;
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
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.39066F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.42506F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.00737F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.07617F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.739558F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.33907F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.88206F));
            this.tableLayoutPanel2.Controls.Add(this.CurrentScoreLabel, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.CurrentTimeLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.CurrentScoreHeadingLabel, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.PlayButton, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.RestartButton, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.CurrentTimeHeadingLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.SpeedDropDownList, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(255, 36);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(814, 41);
            this.tableLayoutPanel2.TabIndex = 15;
            // 
            // SimulationPanel
            // 
            this.SimulationPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SimulationPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.SimulationPanel.Controls.Add(this.SimulationLabel);
            this.SimulationPanel.Controls.Add(this.SimulationProgress);
            this.SimulationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SimulationPanel.Location = new System.Drawing.Point(252, 113);
            this.SimulationPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SimulationPanel.Name = "SimulationPanel";
            this.SimulationPanel.Size = new System.Drawing.Size(820, 551);
            this.SimulationPanel.TabIndex = 2;
            // 
            // SimulationLabel
            // 
            this.SimulationLabel.AutoSize = true;
            this.SimulationLabel.BackColor = System.Drawing.Color.Transparent;
            this.SimulationLabel.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SimulationLabel.Location = new System.Drawing.Point(344, 208);
            this.SimulationLabel.Name = "SimulationLabel";
            this.SimulationLabel.Size = new System.Drawing.Size(141, 29);
            this.SimulationLabel.TabIndex = 1;
            this.SimulationLabel.Text = "Simulating";
            this.SimulationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SimulationLabel.Visible = false;
            // 
            // SimulationProgress
            // 
            this.SimulationProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SimulationProgress.Location = new System.Drawing.Point(-1, 254);
            this.SimulationProgress.Name = "SimulationProgress";
            this.SimulationProgress.Size = new System.Drawing.Size(821, 0);
            this.SimulationProgress.TabIndex = 0;
            this.SimulationProgress.Visible = false;
            // 
            // PlaySlider
            // 
            this.PlaySlider.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlaySlider.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.PlaySlider.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.PlaySlider.Location = new System.Drawing.Point(255, 83);
            this.PlaySlider.Max = 1500;
            this.PlaySlider.Name = "PlaySlider";
            this.PlaySlider.Size = new System.Drawing.Size(814, 27);
            this.PlaySlider.TabIndex = 20;
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SimulatorForm";
            this.Text = "Football AI Game";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.SimulationPanel.ResumeLayout(false);
            this.SimulationPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartMatchButton;
        private System.Windows.Forms.ListBox AiListBox;
        private System.Windows.Forms.ListBox ErrorsListBox;
        private System.Windows.Forms.ListBox GoalsListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label FinalShotsOnTargetLabel;
        private System.Windows.Forms.Label FinalShotsLabel;
        private System.Windows.Forms.Panel panel1;
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
        private DoubleBufferedPanel SimulationPanel;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private Panel panel2;
        private Label HeadingLabel;
    }
}

