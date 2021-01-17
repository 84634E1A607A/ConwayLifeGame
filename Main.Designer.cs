
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
            System.Windows.Forms.ToolStripSeparator EditSeparator1;
            this.MainFormMenu = new System.Windows.Forms.MenuStrip();
            this.MainMenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.FileNewWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.FileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.FileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.FileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.EditShowWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.EditCreateSolid = new System.Windows.Forms.ToolStripMenuItem();
            this.EditCreateRandom = new System.Windows.Forms.ToolStripMenuItem();
            this.EditDeleteRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.EditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.EditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.ClacTimer = new System.Windows.Forms.Timer(this.components);
            this.PaintTimer = new System.Windows.Forms.Timer(this.components);
            EditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MainFormMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // EditSeparator1
            // 
            EditSeparator1.Name = "EditSeparator1";
            EditSeparator1.Size = new System.Drawing.Size(209, 6);
            // 
            // MainFormMenu
            // 
            this.MainFormMenu.AllowDrop = true;
            this.MainFormMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainMenuFile,
            this.MainMenuEdit,
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
            this.FileOpen.Click += new System.EventHandler(this.FileOpen_Click);
            // 
            // FileSave
            // 
            this.FileSave.Name = "FileSave";
            this.FileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.FileSave.ShowShortcutKeys = false;
            this.FileSave.Size = new System.Drawing.Size(145, 22);
            this.FileSave.Text = "&Save";
            this.FileSave.Click += new System.EventHandler(this.FileSave_Click);
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
            // MainMenuEdit
            // 
            this.MainMenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditShowWindow,
            this.EditCreateSolid,
            this.EditCreateRandom,
            this.EditDeleteRegion,
            EditSeparator1,
            this.EditCopy,
            this.EditPaste});
            this.MainMenuEdit.Name = "MainMenuEdit";
            this.MainMenuEdit.Size = new System.Drawing.Size(42, 21);
            this.MainMenuEdit.Text = "&Edit";
            // 
            // EditShowWindow
            // 
            this.EditShowWindow.Name = "EditShowWindow";
            this.EditShowWindow.Size = new System.Drawing.Size(212, 22);
            this.EditShowWindow.Text = "&Show Control Window";
            this.EditShowWindow.Click += new System.EventHandler(this.EditShowWindow_Click);
            // 
            // EditCreateSolid
            // 
            this.EditCreateSolid.Name = "EditCreateSolid";
            this.EditCreateSolid.Size = new System.Drawing.Size(212, 22);
            this.EditCreateSolid.Text = "Create &Solid Region";
            this.EditCreateSolid.Click += new System.EventHandler(this.EditCreateSolid_Click);
            // 
            // EditCreateRandom
            // 
            this.EditCreateRandom.Name = "EditCreateRandom";
            this.EditCreateRandom.Size = new System.Drawing.Size(212, 22);
            this.EditCreateRandom.Text = "Create &Random Region";
            this.EditCreateRandom.Click += new System.EventHandler(this.EditCreateRandom_Click);
            // 
            // EditDeleteRegion
            // 
            this.EditDeleteRegion.Name = "EditDeleteRegion";
            this.EditDeleteRegion.Size = new System.Drawing.Size(212, 22);
            this.EditDeleteRegion.Text = "&Delete Solid Region";
            this.EditDeleteRegion.Click += new System.EventHandler(this.EditDeleteRegion_Click);
            // 
            // EditCopy
            // 
            this.EditCopy.Name = "EditCopy";
            this.EditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.EditCopy.ShowShortcutKeys = false;
            this.EditCopy.Size = new System.Drawing.Size(212, 22);
            this.EditCopy.Text = "&Copy";
            this.EditCopy.Click += new System.EventHandler(this.EditCopy_Click);
            // 
            // EditPaste
            // 
            this.EditPaste.Name = "EditPaste";
            this.EditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.EditPaste.ShowShortcutKeys = false;
            this.EditPaste.Size = new System.Drawing.Size(212, 22);
            this.EditPaste.Text = "&Paste";
            this.EditPaste.Click += new System.EventHandler(this.EditPaste_Click);
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
            this.MainPanel.Size = new System.Drawing.Size(711, 393);
            this.MainPanel.TabIndex = 1;
            this.MainPanel.SizeChanged += new System.EventHandler(this.MainPanel_SizeChanged);
            this.MainPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainPanel_MouseDown);
            this.MainPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainPanel_MouseMove);
            this.MainPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainPanel_MouseUp);
            this.MainPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MainPanel_MouseWheel);
            // 
            // ClacTimer
            // 
            this.ClacTimer.Tick += new System.EventHandler(this.ClacTimer_Tick);
            // 
            // PaintTimer
            // 
            this.PaintTimer.Enabled = true;
            this.PaintTimer.Interval = 20;
            this.PaintTimer.Tick += new System.EventHandler(this.PaintTimer_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 420);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.MainFormMenu);
            this.MainMenuStrip = this.MainFormMenu;
            this.Name = "Main";
            this.Text = "Conway\'s Life Game";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
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
        private System.Windows.Forms.ToolStripMenuItem MainMenuEdit;
        private System.Windows.Forms.ToolStripMenuItem EditShowWindow;
        private System.Windows.Forms.ToolStripMenuItem EditCreateSolid;
        private System.Windows.Forms.ToolStripMenuItem EditCreateRandom;
        private System.Windows.Forms.ToolStripMenuItem EditDeleteRegion;
        private System.Windows.Forms.ToolStripMenuItem EditCopy;
        private System.Windows.Forms.ToolStripMenuItem EditPaste;
        private System.Windows.Forms.ToolStripMenuItem MainMenuHelp;
        private System.Windows.Forms.ToolStripMenuItem HelpAbout;
        private System.Windows.Forms.ToolStripMenuItem HelpHelp;
        public System.Windows.Forms.Panel MainPanel;
        public System.Windows.Forms.Timer ClacTimer;
        private System.Windows.Forms.Timer PaintTimer;
    }
}

