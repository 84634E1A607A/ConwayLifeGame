
namespace ConwayLifeGame
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainFormMenu = new System.Windows.Forms.MenuStrip();
            this.MainMenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.FileNewWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.FileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.FileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.FileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsShowWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsCreateSolid = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsCreateRandom = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsDeleteRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.ClacTimer = new System.Windows.Forms.Timer(this.components);
            this.MainFormMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainFormMenu
            // 
            this.MainFormMenu.AllowDrop = true;
            this.MainFormMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainMenuFile,
            this.MainMenuOptions,
            this.MainMenuHelp});
            this.MainFormMenu.Location = new System.Drawing.Point(0, 0);
            this.MainFormMenu.Name = "MainFormMenu";
            this.MainFormMenu.Size = new System.Drawing.Size(711, 25);
            this.MainFormMenu.TabIndex = 0;
            this.MainFormMenu.Text = "Menu";
            // 
            // MainMenuFile
            // 
            this.MainMenuFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MainMenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileNewWindow,
            this.FileOpen,
            this.FileSave,
            this.FileExit});
            this.MainMenuFile.Name = "MainMenuFile";
            this.MainMenuFile.Size = new System.Drawing.Size(39, 21);
            this.MainMenuFile.Text = "&File";
            // 
            // FileNewWindow
            // 
            this.FileNewWindow.Name = "FileNewWindow";
            this.FileNewWindow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.FileNewWindow.ShowShortcutKeys = false;
            this.FileNewWindow.Size = new System.Drawing.Size(145, 22);
            this.FileNewWindow.Text = "&New Window";
            this.FileNewWindow.Click += new System.EventHandler(this.FileNewWindow_Click);
            // 
            // FileOpen
            // 
            this.FileOpen.Name = "FileOpen";
            this.FileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.FileOpen.ShowShortcutKeys = false;
            this.FileOpen.Size = new System.Drawing.Size(145, 22);
            this.FileOpen.Text = "&Open";
            // 
            // FileSave
            // 
            this.FileSave.Name = "FileSave";
            this.FileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.FileSave.ShowShortcutKeys = false;
            this.FileSave.Size = new System.Drawing.Size(145, 22);
            this.FileSave.Text = "&Save";
            // 
            // FileExit
            // 
            this.FileExit.Name = "FileExit";
            this.FileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.FileExit.ShowShortcutKeys = false;
            this.FileExit.Size = new System.Drawing.Size(145, 22);
            this.FileExit.Text = "E&xit";
            this.FileExit.Click += new System.EventHandler(this.FileExit_Click);
            // 
            // MainMenuOptions
            // 
            this.MainMenuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OptionsShowWindow,
            this.OptionsCreateSolid,
            this.OptionsCreateRandom,
            this.OptionsDeleteRegion});
            this.MainMenuOptions.Name = "MainMenuOptions";
            this.MainMenuOptions.Size = new System.Drawing.Size(66, 21);
            this.MainMenuOptions.Text = "&Options";
            // 
            // OptionsShowWindow
            // 
            this.OptionsShowWindow.Name = "OptionsShowWindow";
            this.OptionsShowWindow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.OptionsShowWindow.Size = new System.Drawing.Size(249, 22);
            this.OptionsShowWindow.Text = "&Show Control Window";
            this.OptionsShowWindow.Click += new System.EventHandler(this.OptionsShowWindow_Click);
            // 
            // OptionsCreateSolid
            // 
            this.OptionsCreateSolid.Name = "OptionsCreateSolid";
            this.OptionsCreateSolid.Size = new System.Drawing.Size(249, 22);
            this.OptionsCreateSolid.Text = "&Create Solid Region";
            // 
            // OptionsCreateRandom
            // 
            this.OptionsCreateRandom.Name = "OptionsCreateRandom";
            this.OptionsCreateRandom.Size = new System.Drawing.Size(249, 22);
            this.OptionsCreateRandom.Text = "Create &Random Region";
            // 
            // OptionsDeleteRegion
            // 
            this.OptionsDeleteRegion.Name = "OptionsDeleteRegion";
            this.OptionsDeleteRegion.Size = new System.Drawing.Size(249, 22);
            this.OptionsDeleteRegion.Text = "&Delete Solid Region";
            // 
            // MainMenuHelp
            // 
            this.MainMenuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HelpAbout,
            this.HelpHelp});
            this.MainMenuHelp.Name = "MainMenuHelp";
            this.MainMenuHelp.Size = new System.Drawing.Size(47, 21);
            this.MainMenuHelp.Text = "&Help";
            // 
            // HelpAbout
            // 
            this.HelpAbout.Name = "HelpAbout";
            this.HelpAbout.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.H)));
            this.HelpAbout.Size = new System.Drawing.Size(191, 22);
            this.HelpAbout.Text = "&About";
            this.HelpAbout.Click += new System.EventHandler(this.HelpAbout_Click);
            // 
            // HelpHelp
            // 
            this.HelpHelp.Name = "HelpHelp";
            this.HelpHelp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.HelpHelp.Size = new System.Drawing.Size(191, 22);
            this.HelpHelp.Text = "&Help";
            this.HelpHelp.Click += new System.EventHandler(this.HelpHelp_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPanel.Location = new System.Drawing.Point(0, 28);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(711, 381);
            this.MainPanel.TabIndex = 1;
            this.MainPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.MainPanel_Paint);
            this.MainPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainPanel_MouseDown);
            // 
            // ClacTimer
            // 
            this.ClacTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 421);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.MainFormMenu);
            this.MainMenuStrip = this.MainFormMenu;
            this.Name = "Main";
            this.Text = "Conway\'s Life Game";
            this.Load += new System.EventHandler(this.Main_Load);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.MainFormMenu.ResumeLayout(false);
            this.MainFormMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainFormMenu;
        private System.Windows.Forms.ToolStripMenuItem MainMenuFile;
        private System.Windows.Forms.ToolStripMenuItem FileNewWindow;
        private System.Windows.Forms.ToolStripMenuItem FileOpen;
        private System.Windows.Forms.ToolStripMenuItem FileSave;
        private System.Windows.Forms.ToolStripMenuItem FileExit;
        private System.Windows.Forms.ToolStripMenuItem MainMenuOptions;
        private System.Windows.Forms.ToolStripMenuItem OptionsShowWindow;
        private System.Windows.Forms.ToolStripMenuItem OptionsCreateSolid;
        private System.Windows.Forms.ToolStripMenuItem OptionsCreateRandom;
        private System.Windows.Forms.ToolStripMenuItem OptionsDeleteRegion;
        private System.Windows.Forms.ToolStripMenuItem MainMenuHelp;
        private System.Windows.Forms.ToolStripMenuItem HelpAbout;
        private System.Windows.Forms.ToolStripMenuItem HelpHelp;
        public System.Windows.Forms.Panel MainPanel;
        public System.Windows.Forms.Timer ClacTimer;
    }
}

