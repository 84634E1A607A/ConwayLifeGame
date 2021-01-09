using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Map.Initialize();
            Program.control = new Control();

            /*  PaintTools Init  */
            {
                PaintTools.bkgndPen = new Pen(Color.FromArgb(0xFF, 0x88, 0x88, 0x88), 1);
                PaintTools.bkgndBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
                PaintTools.mapPicBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
                PaintTools.selectRectPen = new Pen(Color.FromArgb(0xAA, Color.DeepSkyBlue));
                PaintTools.selectRectBrush = new SolidBrush(Color.FromArgb(0x55, Color.CadetBlue));
                PaintTools.selectCellPen = new Pen(Color.DarkGreen, 3)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Custom,
                    DashPattern = new float[] { 1, 1 }
                };
                PaintTools.copyPen = new Pen(Color.DarkGreen, 3);
                PaintTools.copyBrush = new SolidBrush(Color.FromArgb(0x33, Color.ForestGreen));
                PaintTools.mainPicBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
                PaintTools.graphics = Graphics.FromImage(PaintTools.mainPicBitmap);
                PaintTools.paintThread = new Thread(new ThreadStart(PaintThread));
                PaintTools.paintThread.Start();
            }
        }

        private void HelpAbout_Click(object sender, EventArgs e)
        {
            About aboutDlg = new About();
            aboutDlg.ShowDialog();
        }

        private void HelpHelp_Click(object sender, EventArgs e)
        {
            Help helpDlg = new Help();
            helpDlg.ShowDialog();
        }

        private void FileNewWindow_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }

        private void EditShowWindow_Click(object sender, EventArgs e)
        {
            Program.control.Show();
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static class PaintTools
        {
            public static Bitmap mainPicBitmap;

            public static Bitmap mapPicBitmap;

            public static Graphics graphics;

            public static Pen bkgndPen;
            public static Bitmap bkgndBitmap;
            public static int bkgndBitmapScale;

            public static Pen selectRectPen;
            public static SolidBrush selectRectBrush;

            public static Pen selectCellPen;

            public static Pen copyPen;
            public static SolidBrush copyBrush;

            public static Thread paintThread;
        }

        public static Bitmap MapBitmap { get { return PaintTools.mapPicBitmap; } set { PaintTools.mapPicBitmap = value; } }

        private void MainPictureBox_Paint()
        {
            Size size = MainPictureBox.Size;
            int mid_x = size.Width / 2, mid_y = size.Height / 2;

            // bkgnd Bitmap Init
            if (PaintTools.bkgndBitmap.Size != size || PaintTools.bkgndBitmapScale != Map.Scale)
            {
                PaintTools.bkgndBitmap.Dispose();
                PaintTools.bkgndBitmap = new Bitmap(size.Width, size.Height);
                PaintTools.bkgndBitmapScale = Map.Scale;
                Graphics bitmapGraphics = Graphics.FromImage(PaintTools.bkgndBitmap);
                /*  lines in bkgndBitmap */
                for (int i = mid_x % PaintTools.bkgndBitmapScale; i <= size.Width; i += PaintTools.bkgndBitmapScale)
                    bitmapGraphics.DrawLine(PaintTools.bkgndPen, i, 0, i, size.Height);
                for (int i = mid_y % PaintTools.bkgndBitmapScale; i <= size.Height; i += PaintTools.bkgndBitmapScale)
                    bitmapGraphics.DrawLine(PaintTools.bkgndPen, 0, i, size.Width, i);
                bitmapGraphics.Dispose();
            }

            // Main Bitmap Init
            if (PaintTools.mainPicBitmap.Size != size)
            {
                PaintTools.graphics.Dispose();
                PaintTools.mainPicBitmap.Dispose();
                PaintTools.mainPicBitmap = new Bitmap(size.Width, size.Height);
                PaintTools.graphics = Graphics.FromImage(PaintTools.mainPicBitmap);
            }

            PaintTools.graphics.Clear(BackColor);

            /*  lines   */
            PaintTools.graphics.DrawImage(PaintTools.bkgndBitmap, 0, 0);

            /*  blocks  */
            PaintTools.graphics.DrawImage(PaintTools.mapPicBitmap, 0, 0);

            /*  select  */
            {
                int l = Math.Min(Map.MouseInfo.select_first.X, Map.MouseInfo.select_second.X);
                int r = Math.Max(Map.MouseInfo.select_first.X, Map.MouseInfo.select_second.X);
                int t = Math.Min(Map.MouseInfo.select_first.Y, Map.MouseInfo.select_second.Y);
                int b = Math.Max(Map.MouseInfo.select_first.Y, Map.MouseInfo.select_second.Y);
                /*  select (rect)  */
                if (r - l != 0 || b - t != 0)
                {
                    Rectangle rect = new Rectangle(l, t, r - l, b - t);
                    PaintTools.graphics.FillRectangle(PaintTools.selectRectBrush, rect);
                    PaintTools.graphics.DrawRectangle(PaintTools.selectRectPen, rect);
                }
                /*  select (cell)  */
                else if (!Map.MouseInfo.select_first.IsEmpty)
                {
                    int rl = (l - mid_x % Map.Scale) / Map.Scale * Map.Scale + mid_x % Map.Scale;
                    int rt = (t - mid_y % Map.Scale) / Map.Scale * Map.Scale + mid_y % Map.Scale;
                    Rectangle rect = new Rectangle(rl, rt, Map.Scale, Map.Scale);
                    PaintTools.graphics.DrawRectangle(PaintTools.selectCellPen, rect);
                }
            }

            /*  copy  */
            if (Map.CopyInfo.state)
            {
                int l = Math.Min(Map.CopyInfo.first.X, Map.CopyInfo.second.X);
                int r = Math.Max(Map.CopyInfo.first.X, Map.CopyInfo.second.X);
                int t = Math.Min(Map.CopyInfo.first.Y, Map.CopyInfo.second.Y);
                int b = Math.Max(Map.CopyInfo.first.Y, Map.CopyInfo.second.Y);
                Rectangle rect = new Rectangle((l - Map.XPivot) * Map.Scale + mid_x, (t - Map.YPivot) * Map.Scale + mid_y, (r - l + 1) * Map.Scale, (b - t + 1) * Map.Scale);
                PaintTools.graphics.DrawRectangle(PaintTools.copyPen, rect);
                PaintTools.graphics.FillRectangle(PaintTools.copyBrush, rect);
            }

            MainPictureBox.Image = PaintTools.mainPicBitmap;
        }

        private void PaintThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(5);
                    try { MainPictureBox_Paint(); }
                    catch (InvalidOperationException) { Thread.Sleep(20); }
                }
            }
            catch (ArgumentException) { return; }
        }

        private void MainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) MainPictureBox_LButtonDown(e);
            else if (e.Button == MouseButtons.Right) MainPictureBox_RButtonDown(e);
            Map.Draw();
        }

        private void MainPictureBox_LButtonDown(MouseEventArgs e)
        {
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (e.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot;
            switch (Map.MouseInfo.state)
            {
                case Map.MouseState.click:
                    {
                        Map.Change(xc, yc);
                        break;
                    }
                case Map.MouseState.pen:
                    {
                        Map.Change(xc, yc);
                        Map.MouseInfo.previous = new Point(xc, yc);
                        break;
                    }
                case Map.MouseState.eraser:
                    {
                        Map.MouseInfo.previous = new Point(xc, yc);
                        Map.Change(xc, yc, 2);
                        break;
                    }
                case Map.MouseState.drag:
                    {
                        Map.MouseInfo.previous = new Point(xc, yc);
                        break;
                    }
                case Map.MouseState.select:
                    {
                        if (Map.Started) Program.control.StartStop_Click(null, null);
                        Map.MouseInfo.select_first = new Point(e.X, e.Y);
                        Map.MouseInfo.select_second = new Point(e.X, e.Y);
                        break;
                    }
            }
            Map.Draw();
        }

        private void MainPictureBox_RButtonDown(MouseEventArgs e)
        {
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (e.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot;
            Map.AddPreset(xc, yc);
            Map.Draw();
        }

        private void MainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Map.MouseInfo.state != Map.MouseState.click)
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((e.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((e.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point pcur = new Point(xc, yc);
                switch (Map.MouseInfo.state)
                {
                    case Map.MouseState.drag:
                        {
                            Program.control.XPivot.Value = Map.MouseInfo.previous.X - xc + Map.XPivot;
                            Program.control.YPivot.Value = Map.MouseInfo.previous.Y - yc + Map.YPivot;
                            break;
                        }
                    case Map.MouseState.pen:
                        {
                            {
                                Point s = (pcur.X <= Map.MouseInfo.previous.X) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.Y - s.Y) / ((double)t.X - s.X);
                                for (int i = s.X; i <= t.X; i++)
                                    Map.Change(i, (int)(s.Y + ((double)i - s.X) * k), 1);
                            }
                            {
                                Point s = (pcur.Y <= Map.MouseInfo.previous.Y) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.X - s.X) / ((double)t.Y - s.Y);
                                for (int i = s.Y; i <= t.Y; i++)
                                    Map.Change((int)(s.X + ((double)i - s.Y) * k), i, 1);
                            }
                            Map.MouseInfo.previous = pcur;
                            Map.Draw();
                            break;
                        }
                    case Map.MouseState.eraser:
                        {
                            {
                                Point s = (pcur.X <= Map.MouseInfo.previous.X) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.Y - s.Y) / ((double)t.X - s.X);
                                for (int i = s.X; i <= t.X; i++)
                                    Map.Change(i, (int)(s.Y + ((double)i - s.X) * k), 2);
                            }
                            {
                                Point s = (pcur.Y <= Map.MouseInfo.previous.Y) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.X - s.X) / ((double)t.Y - s.Y);
                                for (int i = s.Y; i <= t.Y; i++)
                                    Map.Change((int)(s.X + ((double)i - s.Y) * k), i, 2);
                            }
                            Map.MouseInfo.previous = pcur;
                            Map.Draw();
                            break;
                        }
                    case Map.MouseState.select:
                        {
                            Map.MouseInfo.select_second = new Point(e.X, e.Y);
                            break;
                        }
                }
            }
        }

        private void MainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (Map.AddRgnInfo.state != Map.AddRegionState.normal)
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
                Map.AddRgnInfo.state = Map.AddRegionState.normal;
                
                switch (Map.AddRgnInfo.lastMouseState)
                {
                    case Map.MouseState.click:
                        Program.control.MouseStateClick.Checked = true;
                        break;
                    case Map.MouseState.pen:
                        Program.control.MouseStatePen.Checked = true;
                        break;
                    case Map.MouseState.eraser:
                        Program.control.MouseStateEraser.Checked = true;
                        break;
                    case Map.MouseState.drag:
                        Program.control.MouseStateDrag.Checked = true;
                        break;
                    default: break;
                }
            }
            Map.Draw();
        }

        private void MainPictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int i = (int)Program.control.MapScale.Value;
            if (!( i <= 2) || !(e.Delta < 0))
            {
                i += e.Delta / 40;
                if (i < 2) i = 3;
                if (i > 999) i = 999;
            }
            Program.control.MapScale.Value = i;
            Map.Draw();
        }

        private void ClacTimer_Tick(object sender, EventArgs e)
        {
            Map.Calc();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            int move_length = 40;
            if (e.Control || e.Alt || e.Shift) { e.Handled = false; return; }
            switch (e.KeyCode)
            {
                case Keys.B:
                    {
                        Map.KeybdInputState = Map.KeyboardInputState.bulitin;
                        break;
                    }
                case Keys.D:
                    {
                        Map.KeybdInputState = Map.KeyboardInputState.direction;
                        break;
                    }
                case Keys.C:
                    {
                        EditShowWindow_Click(null, null);
                        break;
                    }
                case Keys.Space:
                    {
                        Program.control.StartStop_Click(null, null);
                        break;
                    }
                case Keys.Delete:
                    {
                        Program.control.Reset_Click(null, null);
                        break;
                    }
                case Keys.Left:
                    {
                        Program.control.XPivot.Value -= move_length / Map.Scale;
                        break;
                    }
                case Keys.Right:
                    {
                        Program.control.XPivot.Value += move_length / Map.Scale;
                        break;
                    }
                case Keys.Up:
                    {
                        Program.control.YPivot.Value -= move_length / Map.Scale;
                        break;
                    }
                case Keys.Down:
                    {
                        Program.control.YPivot.Value += move_length / Map.Scale;
                        break;
                    }
                case Keys.Oemplus:
                    {
                        try { Program.control.Timer.Value -= 5; }
                        catch (ArgumentOutOfRangeException)
                        { Program.control.Timer.Value = Program.control.Timer.Minimum; }
                        break;
                    }
                case Keys.OemMinus:
                    {
                        try { Program.control.Timer.Value += 5; }
                        catch (ArgumentOutOfRangeException)
                        { Program.control.Timer.Value = Program.control.Timer.Maximum; }
                        break;
                    }
                default:
                    {
                        if (!(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)) { e.Handled = false; return; }

                        switch (Map.KeybdInputState)
                        {
                            case Map.KeyboardInputState.normal: { e.Handled = false; return; }
                            case Map.KeyboardInputState.bulitin:
                                {
                                    Program.control.PresetSelect.Value = e.KeyCode - Keys.D0;
                                    break;
                                }
                            case Map.KeyboardInputState.direction:
                                {
                                    Program.control.DirectionSelect.Value = e.KeyCode - Keys.D0;
                                    break;
                                }
                        }
                        Map.KeybdInputState = Map.KeyboardInputState.normal;
                        break;
                    }
            }
            Map.Draw();
            e.Handled = true;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            PaintTools.graphics.Dispose();
            PaintTools.bkgndPen.Dispose();
            PaintTools.bkgndBitmap.Dispose();
            PaintTools.mainPicBitmap.Dispose();
            PaintTools.copyPen.Dispose();
            PaintTools.copyBrush.Dispose();
            PaintTools.mapPicBitmap.Dispose();
            PaintTools.selectCellPen.Dispose();
            PaintTools.selectRectPen.Dispose();
            PaintTools.selectRectBrush.Dispose();
        }

        private void EditCreateRandom_Click(object sender, EventArgs e)
        {
            Map.AddRgnInfo.state = Map.AddRegionState.random;
            if (Map.MouseInfo.select_first.IsEmpty)
            {
                Map.AddRgnInfo.lastMouseState = Map.MouseInfo.state;
                Program.control.MouseStateSelect.Checked = true;
            }
            else
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
                Map.AddRgnInfo.state = Map.AddRegionState.normal;
            }
            Map.Draw();
        }

        private void EditCreateSolid_Click(object sender, EventArgs e)
        {
            Map.AddRgnInfo.state = Map.AddRegionState.insert;
            if (Map.MouseInfo.select_first.IsEmpty)
            {
                Map.AddRgnInfo.lastMouseState = Map.MouseInfo.state;
                Program.control.MouseStateSelect.Checked = true;
            }
            else
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
                Map.AddRgnInfo.state = Map.AddRegionState.normal;
            }
            Map.Draw();
        }

        private void EditDeleteRegion_Click(object sender, EventArgs e)
        {
            Map.AddRgnInfo.state = Map.AddRegionState.delete;
            if (Map.MouseInfo.select_first.IsEmpty)
            {
                Map.AddRgnInfo.lastMouseState = Map.MouseInfo.state;
                Program.control.MouseStateSelect.Checked = true;
            }
            else
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
                Map.AddRgnInfo.state = Map.AddRegionState.normal;
            }
            Map.Draw();
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "JSON Life File|*.lfs|Life File|*.lf||",
                DefaultExt = ".lfs"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fname = openFileDialog.FileName;
                if (fname.EndsWith(".lfs")) Map.LoadLFS(fname);
                if (fname.EndsWith(".lf")) Map.LoadLF(fname);
            }
            Map.Draw();
        }

        private void FileSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Life File|*.lfs||",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                Map.DumpLFS(saveFileDialog.FileName);
            Map.Draw();
        }

        private void EditCopy_Click(object sender, EventArgs e)
        {
            if (Map.MouseInfo.select_first.IsEmpty && Map.MouseInfo.select_second.IsEmpty) return;
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
            int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
            Map.CopyInfo.first = new Point(xc, yc);
            xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
            yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
            Map.CopyInfo.second = new Point(xc, yc);
            Map.CopyInfo.state = true;
            Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
        }

        private void EditPaste_Click(object sender, EventArgs e)
        {
            if (!Map.CopyInfo.state) return;
            if (Map.MouseInfo.select_first != Map.MouseInfo.select_second) return; 
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
            int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
            Map.Paste(xc, yc);
            Map.CopyInfo.state = false;
            Map.Draw();
        }
    }
}
