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
                paintTools.bkgndPen = new Pen(Color.FromArgb(0xFF, 0x88, 0x88, 0x88), 1);
                paintTools.bkgndBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
                paintTools.mapPicBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
                paintTools.selectRectPen = new Pen(Color.FromArgb(0xAA, Color.DeepSkyBlue));
                paintTools.selectRectBrush = new SolidBrush(Color.FromArgb(0x55, Color.CadetBlue));
                paintTools.selectCellPen = new Pen(Color.DarkGreen, 3)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Custom,
                    DashPattern = new float[] { 1, 1 }
                };
                paintTools.copyPen = new Pen(Color.DarkGreen, 3);
                paintTools.copyBrush = new SolidBrush(Color.FromArgb(0x33, Color.ForestGreen));
                paintTools.mainPicBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
                paintTools.graphics = Graphics.FromImage(paintTools.mainPicBitmap);
                paintTools.paintThread = new Thread(new ThreadStart(PaintThread));
                paintTools.paintThread.Start();
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

        private struct PictureBoxPaintTools
        {
            public Bitmap mainPicBitmap;

            public Bitmap mapPicBitmap;

            public Graphics graphics;

            public Pen bkgndPen;
            public Bitmap bkgndBitmap;
            public int bkgndBitmapScale;

            public Pen selectRectPen;
            public SolidBrush selectRectBrush;

            public Pen selectCellPen;

            public Pen copyPen;
            public SolidBrush copyBrush;

            public Thread paintThread;
        }
        PictureBoxPaintTools paintTools;

        public Bitmap MapBitmap { get { return paintTools.mapPicBitmap; } set { paintTools.mapPicBitmap = value; } }

        private void MainPictureBox_Paint()
        {
            Size size = MainPictureBox.Size;
            int mid_x = size.Width / 2, mid_y = size.Height / 2;

            // bkgnd Bitmap Init
            if (paintTools.bkgndBitmap.Size != size || paintTools.bkgndBitmapScale != Map.scale)
            {
                paintTools.bkgndBitmap.Dispose();
                paintTools.bkgndBitmap = new Bitmap(size.Width, size.Height);
                paintTools.bkgndBitmapScale = Map.scale;
                Graphics bitmapGraphics = Graphics.FromImage(paintTools.bkgndBitmap);
                /*  lines in bkgndBitmap */
                for (int i = mid_x % paintTools.bkgndBitmapScale; i <= size.Width; i += paintTools.bkgndBitmapScale)
                    bitmapGraphics.DrawLine(paintTools.bkgndPen, i, 0, i, size.Height);
                for (int i = mid_y % paintTools.bkgndBitmapScale; i <= size.Height; i += paintTools.bkgndBitmapScale)
                    bitmapGraphics.DrawLine(paintTools.bkgndPen, 0, i, size.Width, i);
                bitmapGraphics.Dispose();
            }

            // Main Bitmap Init
            if (paintTools.mainPicBitmap.Size != size)
            {
                paintTools.graphics.Dispose();
                paintTools.mainPicBitmap.Dispose();
                paintTools.mainPicBitmap = new Bitmap(size.Width, size.Height);
                paintTools.graphics = Graphics.FromImage(paintTools.mainPicBitmap);
            }

            paintTools.graphics.Clear(BackColor);

            /*  lines   */
            paintTools.graphics.DrawImage(paintTools.bkgndBitmap, 0, 0);

            /*  blocks  */
            paintTools.graphics.DrawImage(paintTools.mapPicBitmap, 0, 0);

            /*  select  */
            {
                int l = Math.Min(Map.mouse_info.select_first.X, Map.mouse_info.select_second.X);
                int r = Math.Max(Map.mouse_info.select_first.X, Map.mouse_info.select_second.X);
                int t = Math.Min(Map.mouse_info.select_first.Y, Map.mouse_info.select_second.Y);
                int b = Math.Max(Map.mouse_info.select_first.Y, Map.mouse_info.select_second.Y);
                /*  select (rect)  */
                if (r - l != 0 || b - t != 0)
                {
                    Rectangle rect = new Rectangle(l, t, r - l, b - t);
                    paintTools.graphics.FillRectangle(paintTools.selectRectBrush, rect);
                    paintTools.graphics.DrawRectangle(paintTools.selectRectPen, rect);
                }
                /*  select (cell)  */
                else if (!Map.mouse_info.select_first.IsEmpty)
                {
                    int rl = (l - mid_x % Map.scale) / Map.scale * Map.scale + mid_x % Map.scale;
                    int rt = (t - mid_y % Map.scale) / Map.scale * Map.scale + mid_y % Map.scale;
                    Rectangle rect = new Rectangle(rl, rt, Map.scale, Map.scale);
                    paintTools.graphics.DrawRectangle(paintTools.selectCellPen, rect);
                }
            }

            /*  copy  */
            if (Map.copy_info.state)
            {
                int l = Math.Min(Map.copy_info.first.X, Map.copy_info.second.X);
                int r = Math.Max(Map.copy_info.first.X, Map.copy_info.second.X);
                int t = Math.Min(Map.copy_info.first.Y, Map.copy_info.second.Y);
                int b = Math.Max(Map.copy_info.first.Y, Map.copy_info.second.Y);
                Rectangle rect = new Rectangle((l - Map.x_pivot) * Map.scale + mid_x, (t - Map.y_pivot) * Map.scale + mid_y, (r - l + 1) * Map.scale, (b - t + 1) * Map.scale);
                paintTools.graphics.DrawRectangle(paintTools.copyPen, rect);
                paintTools.graphics.FillRectangle(paintTools.copyBrush, rect);
            }

            MainPictureBox.Image = paintTools.mainPicBitmap;
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
            int xc = (e.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot;
            switch (Map.mouse_info.state)
            {
                case Map.MouseState.click:
                    {
                        Map.Change(xc, yc);
                        break;
                    }
                case Map.MouseState.pen:
                    {
                        Map.Change(xc, yc);
                        Map.mouse_info.previous = new Point(xc, yc);
                        break;
                    }
                case Map.MouseState.eraser:
                    {
                        Map.mouse_info.previous = new Point(xc, yc);
                        Map.Change(xc, yc, 2);
                        break;
                    }
                case Map.MouseState.drag:
                    {
                        Map.mouse_info.previous = new Point(xc, yc);
                        break;
                    }
                case Map.MouseState.select:
                    {
                        if (Map.started) Program.control.StartStop_Click(null, null);
                        Map.mouse_info.select_first = new Point(e.X, e.Y);
                        Map.mouse_info.select_second = new Point(e.X, e.Y);
                        break;
                    }
            }
            Map.Draw();
        }

        private void MainPictureBox_RButtonDown(MouseEventArgs e)
        {
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (e.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot;
            Map.AddPreset(xc, yc);
            Map.Draw();
        }

        private void MainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Map.mouse_info.state != Map.MouseState.click)
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((e.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                int yc = (int)((e.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point pcur = new Point(xc, yc);
                switch (Map.mouse_info.state)
                {
                    case Map.MouseState.drag:
                        {
                            Program.control.XPivot.Value = Map.mouse_info.previous.X - xc + Map.x_pivot;
                            Program.control.YPivot.Value = Map.mouse_info.previous.Y - yc + Map.y_pivot;
                            break;
                        }
                    case Map.MouseState.pen:
                        {
                            {
                                Point s = (pcur.X <= Map.mouse_info.previous.X) ? pcur : Map.mouse_info.previous, t = (s == pcur) ? Map.mouse_info.previous : pcur;
                                double k = ((double)t.Y - s.Y) / ((double)t.X - s.X);
                                for (int i = s.X; i <= t.X; i++)
                                    Map.Change(i, (int)(s.Y + ((double)i - s.X) * k), 1);
                            }
                            {
                                Point s = (pcur.Y <= Map.mouse_info.previous.Y) ? pcur : Map.mouse_info.previous, t = (s == pcur) ? Map.mouse_info.previous : pcur;
                                double k = ((double)t.X - s.X) / ((double)t.Y - s.Y);
                                for (int i = s.Y; i <= t.Y; i++)
                                    Map.Change((int)(s.X + ((double)i - s.Y) * k), i, 1);
                            }
                            Map.mouse_info.previous = pcur;
                            Map.Draw();
                            break;
                        }
                    case Map.MouseState.eraser:
                        {
                            {
                                Point s = (pcur.X <= Map.mouse_info.previous.X) ? pcur : Map.mouse_info.previous, t = (s == pcur) ? Map.mouse_info.previous : pcur;
                                double k = ((double)t.Y - s.Y) / ((double)t.X - s.X);
                                for (int i = s.X; i <= t.X; i++)
                                    Map.Change(i, (int)(s.Y + ((double)i - s.X) * k), 2);
                            }
                            {
                                Point s = (pcur.Y <= Map.mouse_info.previous.Y) ? pcur : Map.mouse_info.previous, t = (s == pcur) ? Map.mouse_info.previous : pcur;
                                double k = ((double)t.X - s.X) / ((double)t.Y - s.Y);
                                for (int i = s.Y; i <= t.Y; i++)
                                    Map.Change((int)(s.X + ((double)i - s.Y) * k), i, 2);
                            }
                            Map.mouse_info.previous = pcur;
                            Map.Draw();
                            break;
                        }
                    case Map.MouseState.select:
                        {
                            Map.mouse_info.select_second = new Point(e.X, e.Y);
                            break;
                        }
                }
            }
        }

        private void MainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (Map.add_region_info.state != Map.AddRegionState.normal)
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.mouse_info.select_first.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                int yc = (int)((Map.mouse_info.select_first.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.mouse_info.select_second.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                yc = (int)((Map.mouse_info.select_second.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.mouse_info.select_first = Map.mouse_info.select_second = new Point();
                Map.add_region_info.state = Map.AddRegionState.normal;
                
                switch (Map.add_region_info.lastMouseState)
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
                        Map.keyboard_input_state = Map.KeyboardInputState.bulitin;
                        break;
                    }
                case Keys.D:
                    {
                        Map.keyboard_input_state = Map.KeyboardInputState.direction;
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
                        Program.control.XPivot.Value -= move_length / Map.scale;
                        break;
                    }
                case Keys.Right:
                    {
                        Program.control.XPivot.Value += move_length / Map.scale;
                        break;
                    }
                case Keys.Up:
                    {
                        Program.control.YPivot.Value -= move_length / Map.scale;
                        break;
                    }
                case Keys.Down:
                    {
                        Program.control.YPivot.Value += move_length / Map.scale;
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
                        if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                        {
                            try
                            {
                                switch (Map.keyboard_input_state)
                                {
                                    case Map.KeyboardInputState.normal: { throw new Exception(); }
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
                                Map.keyboard_input_state = Map.KeyboardInputState.normal;
                                e.Handled = true;
                                return;
                            }
                            catch (Exception) { }
                        }
                        e.Handled = false;
                        return;
                    }
            }
            Map.Draw();
            e.Handled = true;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            paintTools.graphics.Dispose();
            paintTools.bkgndPen.Dispose();
            paintTools.bkgndBitmap.Dispose();
            paintTools.mainPicBitmap.Dispose();
        }

        private void EditCreateRandom_Click(object sender, EventArgs e)
        {
            Map.add_region_info.state = Map.AddRegionState.random;
            if (Map.mouse_info.select_first.IsEmpty)
            {
                Map.add_region_info.lastMouseState = Map.mouse_info.state;
                Program.control.MouseStateSelect.Checked = true;
            }
            else
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.mouse_info.select_first.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                int yc = (int)((Map.mouse_info.select_first.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.mouse_info.select_second.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                yc = (int)((Map.mouse_info.select_second.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.mouse_info.select_first = Map.mouse_info.select_second = new Point();
                Map.add_region_info.state = Map.AddRegionState.normal;
            }
        }

        private void EditCreateSolid_Click(object sender, EventArgs e)
        {
            Map.add_region_info.state = Map.AddRegionState.insert;
            if (Map.mouse_info.select_first.IsEmpty)
            {
                Map.add_region_info.lastMouseState = Map.mouse_info.state;
                Program.control.MouseStateSelect.Checked = true;
            }
            else
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.mouse_info.select_first.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                int yc = (int)((Map.mouse_info.select_first.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.mouse_info.select_second.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                yc = (int)((Map.mouse_info.select_second.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.mouse_info.select_first = Map.mouse_info.select_second = new Point();
                Map.add_region_info.state = Map.AddRegionState.normal;
            }
        }

        private void EditDeleteRegion_Click(object sender, EventArgs e)
        {
            Map.add_region_info.state = Map.AddRegionState.delete;
            if (Map.mouse_info.select_first.IsEmpty)
            {
                Map.add_region_info.lastMouseState = Map.mouse_info.state;
                Program.control.MouseStateSelect.Checked = true;
            }
            else
            {
                int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
                int xc = (int)((Map.mouse_info.select_first.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                int yc = (int)((Map.mouse_info.select_first.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p1 = new Point(xc, yc);
                xc = (int)((Map.mouse_info.select_second.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
                yc = (int)((Map.mouse_info.select_second.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
                Point p2 = new Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.mouse_info.select_first = Map.mouse_info.select_second = new Point();
                Map.add_region_info.state = Map.AddRegionState.normal;
            }
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
        }

        private void EditCopy_Click(object sender, EventArgs e)
        {
            if (Map.mouse_info.select_first.IsEmpty && Map.mouse_info.select_second.IsEmpty) return;
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (int)((Map.mouse_info.select_first.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
            int yc = (int)((Map.mouse_info.select_first.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
            Map.copy_info.first = new Point(xc, yc);
            xc = (int)((Map.mouse_info.select_second.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
            yc = (int)((Map.mouse_info.select_second.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
            Map.copy_info.second = new Point(xc, yc);
            Map.copy_info.state = true;
            Map.mouse_info.select_first = Map.mouse_info.select_second = new Point();
        }

        private void EditPaste_Click(object sender, EventArgs e)
        {
            if (!Map.copy_info.state) return;
            if (Map.mouse_info.select_first != Map.mouse_info.select_second) return; 
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (int)((Map.mouse_info.select_first.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot);
            int yc = (int)((Map.mouse_info.select_first.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot);
            Map.Paste(xc, yc);
            Map.copy_info.state = false;
        }
    }
}
