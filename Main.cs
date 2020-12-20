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
            bkgPen = new Pen(Color.FromArgb(0xFF, 0x88, 0x88, 0x88), 1);
            bkgBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
            selectPen = new Pen(Color.FromArgb(0xAA, Color.DeepSkyBlue));
            selectBrush = new SolidBrush(Color.FromArgb(0x55, Color.CadetBlue));
            mainPicBitmap = new Bitmap(MainPictureBox.Width, MainPictureBox.Height);
            graphics = Graphics.FromImage(mainPicBitmap);
            paintThread = new Thread(new ThreadStart(PaintThread));
            paintThread.Start();
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

        private void OptionsShowWindow_Click(object sender, EventArgs e)
        {
            Program.control.Show();
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Bitmap mainPicBitmap;
        private Graphics graphics;
        private Pen bkgPen;
        private Bitmap bkgBitmap;
        private Pen selectPen;
        private SolidBrush selectBrush;
        private int bitmapScale;
        private Thread paintThread;

        private void MainPictureBox_Paint()
        {
            Size size = MainPictureBox.Size;
            int mid_x = size.Width / 2, mid_y = size.Height / 2;

            // Bkg Bitmap Init
            if (bkgBitmap.Size != size || bitmapScale != Map.scale)
            {
                bkgBitmap.Dispose();
                bkgBitmap = new Bitmap(size.Width, size.Height);
                Graphics bitmapGraphics = Graphics.FromImage(bkgBitmap);
                /*  lines in bkgBitmap */
                for (int i = mid_x % Map.scale; i <= size.Width; i += Map.scale)
                    bitmapGraphics.DrawLine(bkgPen, i, 0, i, size.Height);
                for (int i = mid_y % Map.scale; i <= size.Height; i += Map.scale)
                    bitmapGraphics.DrawLine(bkgPen, 0, i, size.Width, i);
                bitmapGraphics.Dispose();
                bitmapScale = Map.scale;
            }

            // Main Bitmap Init
            if (mainPicBitmap.Size != size)
            {
                graphics.Dispose();
                mainPicBitmap.Dispose();
                mainPicBitmap = new Bitmap(size.Width, size.Height);
                graphics = Graphics.FromImage(mainPicBitmap);
            }
            
            graphics.Clear(BackColor);

            /*  lines   */
            graphics.DrawImage(bkgBitmap, 0, 0);
            /*  blocks  */
            Map.Draw(graphics, size);
            /*  select  */
            int l = Math.Min(Map.mouse_info.select_first.X, Map.mouse_info.select_second.X); 
            int r = Math.Max(Map.mouse_info.select_first.X, Map.mouse_info.select_second.X); 
            int t = Math.Min(Map.mouse_info.select_first.Y, Map.mouse_info.select_second.Y); 
            int b = Math.Max(Map.mouse_info.select_first.Y, Map.mouse_info.select_second.Y);
            if (r - l != 0 || b - t != 0)
            {
                Rectangle rect = new Rectangle(l, t, r - l, b - t);
                graphics.FillRectangle(selectBrush, rect);
                graphics.DrawRectangle(selectPen, rect);
            }

            MainPictureBox.Image = mainPicBitmap;
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
        }

        private void MainPictureBox_RButtonDown(MouseEventArgs e)
        {
            int mid_x = MainPictureBox.Width / 2, mid_y = MainPictureBox.Height / 2;
            int xc = (e.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot;
            Map.AddBuiltin(xc, yc);
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
                        OptionsShowWindow_Click(null, null);
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
                                            Program.control.BuiltinSelect.Value = e.KeyCode - Keys.D0;
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
            e.Handled = true;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            graphics.Dispose();
            bkgPen.Dispose();
            bkgBitmap.Dispose();
            mainPicBitmap.Dispose();
        }

        private void OptionsCreateRandom_Click(object sender, EventArgs e)
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

        private void OptionsCreateSolid_Click(object sender, EventArgs e)
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

        private void OptionsDeleteRegion_Click(object sender, EventArgs e)
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
    }
}
