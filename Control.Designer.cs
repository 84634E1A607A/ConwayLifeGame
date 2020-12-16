
namespace ConwayLifeGame
{
    partial class Control
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PreviewPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.DirectionSelect = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.BuiltinSelect = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Reset = new System.Windows.Forms.Button();
            this.StartStop = new System.Windows.Forms.Button();
            this.Timer = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.MapScale = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.YPivot = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.XPivot = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DirectionSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuiltinSelect)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Timer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YPivot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XPivot)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PreviewPanel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.DirectionSelect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.BuiltinSelect);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 163);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Builtin controls";
            // 
            // PreviewPanel
            // 
            this.PreviewPanel.Location = new System.Drawing.Point(67, 54);
            this.PreviewPanel.Name = "PreviewPanel";
            this.PreviewPanel.Size = new System.Drawing.Size(162, 102);
            this.PreviewPanel.TabIndex = 0;
            this.PreviewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Preview";
            // 
            // DirectionSelect
            // 
            this.DirectionSelect.Location = new System.Drawing.Point(186, 22);
            this.DirectionSelect.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.DirectionSelect.Name = "DirectionSelect";
            this.DirectionSelect.Size = new System.Drawing.Size(39, 23);
            this.DirectionSelect.TabIndex = 1;
            this.DirectionSelect.ValueChanged += new System.EventHandler(this.DirectionSelect_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(125, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Direction";
            // 
            // BuiltinSelect
            // 
            this.BuiltinSelect.Location = new System.Drawing.Point(67, 22);
            this.BuiltinSelect.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.BuiltinSelect.Name = "BuiltinSelect";
            this.BuiltinSelect.Size = new System.Drawing.Size(39, 23);
            this.BuiltinSelect.TabIndex = 0;
            this.BuiltinSelect.ValueChanged += new System.EventHandler(this.BuiltinSelect_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Builtin";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Reset);
            this.groupBox2.Controls.Add(this.StartStop);
            this.groupBox2.Controls.Add(this.Timer);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.MapScale);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.YPivot);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.XPivot);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(13, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 161);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Map controls";
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(143, 125);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(75, 26);
            this.Reset.TabIndex = 10;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // StartStop
            // 
            this.StartStop.Location = new System.Drawing.Point(20, 125);
            this.StartStop.Name = "StartStop";
            this.StartStop.Size = new System.Drawing.Size(75, 26);
            this.StartStop.TabIndex = 9;
            this.StartStop.Text = "Start / Stop";
            this.StartStop.UseVisualStyleBackColor = true;
            this.StartStop.Click += new System.EventHandler(this.StartStop_Click);
            // 
            // Timer
            // 
            this.Timer.Location = new System.Drawing.Point(183, 92);
            this.Timer.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.Timer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Timer.Name = "Timer";
            this.Timer.Size = new System.Drawing.Size(46, 23);
            this.Timer.TabIndex = 8;
            this.Timer.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.Timer.ValueChanged += new System.EventHandler(this.Timer_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(143, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 17);
            this.label7.TabIndex = 7;
            this.label7.Text = "Timer";
            // 
            // MapScale
            // 
            this.MapScale.Location = new System.Drawing.Point(60, 92);
            this.MapScale.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MapScale.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.MapScale.Name = "MapScale";
            this.MapScale.Size = new System.Drawing.Size(46, 23);
            this.MapScale.TabIndex = 6;
            this.MapScale.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.MapScale.ValueChanged += new System.EventHandler(this.MapScale_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Scale";
            // 
            // YPivot
            // 
            this.YPivot.Hexadecimal = true;
            this.YPivot.Location = new System.Drawing.Point(125, 57);
            this.YPivot.Maximum = new decimal(new int[] {
            268435455,
            0,
            0,
            0});
            this.YPivot.Minimum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.YPivot.Name = "YPivot";
            this.YPivot.Size = new System.Drawing.Size(100, 23);
            this.YPivot.TabIndex = 4;
            this.YPivot.Value = new decimal(new int[] {
            134217728,
            0,
            0,
            0});
            this.YPivot.ValueChanged += new System.EventHandler(this.YPivot_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 17);
            this.label5.TabIndex = 3;
            this.label5.Text = "Y Pivot 0x";
            // 
            // XPivot
            // 
            this.XPivot.Hexadecimal = true;
            this.XPivot.Location = new System.Drawing.Point(125, 24);
            this.XPivot.Maximum = new decimal(new int[] {
            268435455,
            0,
            0,
            0});
            this.XPivot.Minimum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.XPivot.Name = "XPivot";
            this.XPivot.Size = new System.Drawing.Size(100, 23);
            this.XPivot.TabIndex = 2;
            this.XPivot.Value = new decimal(new int[] {
            134217728,
            0,
            0,
            0});
            this.XPivot.ValueChanged += new System.EventHandler(this.XPivot_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "X Pivot 0x";
            // 
            // Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 362);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Control";
            this.Text = "Control Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Control_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DirectionSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuiltinSelect)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Timer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YPivot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XPivot)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown BuiltinSelect;
        private System.Windows.Forms.NumericUpDown DirectionSelect;
        private System.Windows.Forms.NumericUpDown XPivot;
        private System.Windows.Forms.NumericUpDown YPivot;
        private System.Windows.Forms.Panel PreviewPanel;
        private System.Windows.Forms.NumericUpDown MapScale;
        private System.Windows.Forms.NumericUpDown Timer;
        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.Button StartStop;
    }
}