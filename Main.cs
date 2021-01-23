using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Threading.Tasks;

namespace ConwayLifeGame
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Map.Initialize();
            Program.control = new Control();
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
            Program.control.Focus();
            Task.Run( () => Program.SetMainLabel("Control window shown", 1000));
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Program.control.Dispose();
            Application.Exit();
        }

        private static class PaintTools
        {
            public static Factory factory;
            public static RenderTargetProperties renderProps;
            public static HwndRenderTargetProperties hwndProps;
            public static WindowRenderTarget renderTarget;
            public static Brush bkgndPen;
            public static Brush selectRectBrush;
            public static Brush selectRectPen;
            public static Brush selectCellPen;
            public static Brush copyBrush;
            public static Brush copyPen;
            public static PathGeometry bkgndGeometry;
            public static int bkgndScale;
            public static Size2F bkgndSize;

            public static bool Init()
            {
                try {
                    factory = new Factory(FactoryType.SingleThreaded);
                    renderProps = new RenderTargetProperties()
                    {
                        PixelFormat = new PixelFormat { AlphaMode = AlphaMode.Ignore, Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm },
                        Usage = RenderTargetUsage.None,
                        Type = RenderTargetType.Default
                    };
                    hwndProps = new HwndRenderTargetProperties()
                    {
                        Hwnd = Program.main.MainPanel.Handle,
                        PixelSize = new Size2(Program.main.MainPanel.Width, Program.main.MainPanel.Height),
                        PresentOptions = PresentOptions.None
                    };
                    renderTarget = new WindowRenderTarget(factory, renderProps, hwndProps)
                    {
                        AntialiasMode = AntialiasMode.PerPrimitive,
                        DotsPerInch = new Size2F(96, 96)
                    };
                    bkgndPen = new SolidColorBrush(renderTarget, new RawColor4(0.3f, 0.3f, 0.3f, 1.0f));
                    selectRectBrush = new SolidColorBrush(renderTarget, new RawColor4(0x00 / 256.0f, 0x97 / 256.0f, 0xA7 / 256.0f, 0.5f));
                    selectRectPen = new SolidColorBrush(renderTarget, new RawColor4(0x1A / 256.0f, 0x23 / 256.0f, 0x7E / 256.0f, 1));
                    selectCellPen = new SolidColorBrush(renderTarget, new RawColor4(0x1B / 256.0f, 0x5E / 256.0f, 0x20 / 256.0f, 1));
                    copyBrush = new SolidColorBrush(renderTarget, new RawColor4(0x66 / 256.0f, 0xBB / 256.0f, 0x6A / 256.0f, 0.5f));
                    copyPen = selectRectPen;
                    bkgndGeometry = new PathGeometry(factory);
                } catch { return false; }
                return true;
            }
        }

        private void MainPanel_Paint()
        {
            //  Init D2D
            if (PaintTools.renderTarget == null)
            { if (!PaintTools.Init()) return; }

            //  Begin draw
            RenderTarget target = PaintTools.renderTarget;
            target.BeginDraw();
            target.Clear(new RawColor4(1, 1, 1, 1));

            System.Drawing.Size size = MainPanel.Size;
            int mid_x = size.Width / 2, mid_y = size.Height / 2, Scale = Map.Scale;

            //  Lines
            if (PaintTools.bkgndSize != target.Size || PaintTools.bkgndScale != Scale)
            {
                PaintTools.bkgndGeometry.Dispose();
                PathGeometry g = PaintTools.bkgndGeometry = new PathGeometry(PaintTools.factory);
                PaintTools.bkgndSize = target.Size; PaintTools.bkgndScale = Scale;
                GeometrySink s = g.Open();
                for (int i = mid_x % Scale; i <= size.Width; i += Scale)
                {
                    s.BeginFigure(new RawVector2(i, 0), FigureBegin.Hollow);
                    s.AddLine(new RawVector2(i, size.Height));
                    s.EndFigure(FigureEnd.Open);
                }
                for (int i = mid_y % Scale; i <= size.Height; i += Scale)
                {
                    s.BeginFigure(new RawVector2(0, i), FigureBegin.Hollow);
                    s.AddLine(new RawVector2(size.Width, i));
                    s.EndFigure(FigureEnd.Open);
                }
                s.Close();
            }
            float width = .05f * Map.Scale;
            target.DrawGeometry(PaintTools.bkgndGeometry, PaintTools.bkgndPen, width);

            //  Blocks
            Map.Draw(target);

            //  Select
            {
                int l = Math.Min(Map.MouseInfo.select_first.X, Map.MouseInfo.select_second.X);
                int r = Math.Max(Map.MouseInfo.select_first.X, Map.MouseInfo.select_second.X);
                int t = Math.Min(Map.MouseInfo.select_first.Y, Map.MouseInfo.select_second.Y);
                int b = Math.Max(Map.MouseInfo.select_first.Y, Map.MouseInfo.select_second.Y);
                //  Select (rect)  
                if (r - l != 0 || b - t != 0)
                {
                    RawRectangleF rect = new RawRectangleF(l, t, r, b);
                    target.FillRectangle(rect, PaintTools.selectRectBrush);
                    target.DrawRectangle(rect, PaintTools.selectRectPen);
                }
                //  Select (cell)  
                else if (!Map.MouseInfo.select_first.IsEmpty)
                {
                    int rl = (l - mid_x % Map.Scale) / Map.Scale * Map.Scale + mid_x % Map.Scale;
                    int rt = (t - mid_y % Map.Scale) / Map.Scale * Map.Scale + mid_y % Map.Scale;
                    RawRectangleF rect = new RawRectangleF(rl, rt, rl + Map.Scale, rt + Map.Scale);
                    target.DrawRectangle(rect, PaintTools.selectCellPen);
                }
            }

            //  Copy  
            if (Map.CopyInfo.state)
            {
                int l = Math.Min(Map.CopyInfo.first.X, Map.CopyInfo.second.X);
                int r = Math.Max(Map.CopyInfo.first.X, Map.CopyInfo.second.X);
                int t = Math.Min(Map.CopyInfo.first.Y, Map.CopyInfo.second.Y);
                int b = Math.Max(Map.CopyInfo.first.Y, Map.CopyInfo.second.Y);
                RawRectangleF rect = new RawRectangleF((l - Map.XPivot) * Map.Scale + mid_x, (t - Map.YPivot) * Map.Scale + mid_y, (r - Map.XPivot + 1) * Map.Scale + mid_x, (b - Map.YPivot + 1) * Map.Scale + mid_y);
                target.DrawRectangle(rect, PaintTools.copyPen);
                target.FillRectangle(rect, PaintTools.copyBrush);
            }

            target.EndDraw();
        }

        private void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0) MainPanel_LButtonDown(new MouseEventArgs(MouseButtons.Left, 1, e.X, e.Y, 0));
            if ((e.Button & MouseButtons.Right) != 0) MainPanel_RButtonDown(new MouseEventArgs(MouseButtons.Right, 1, e.X, e.Y, 0));
        }

        private void MainPanel_LButtonDown(MouseEventArgs e)
        {
            int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
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
                        Map.MouseInfo.previous = new System.Drawing.Point(xc, yc);
                        break;
                    }
                case Map.MouseState.eraser:
                    {
                        Map.MouseInfo.previous = new System.Drawing.Point(xc, yc);
                        Map.Change(xc, yc, 2);
                        break;
                    }
                case Map.MouseState.drag:
                    {
                        Map.MouseInfo.previous = new System.Drawing.Point(xc, yc);
                        break;
                    }
                case Map.MouseState.select:
                    {
                        if (Map.Started) Program.control.StartStop_Click(null, null);
                        Map.MouseInfo.select_first = new System.Drawing.Point(e.X, e.Y);
                        Map.MouseInfo.select_second = new System.Drawing.Point(e.X, e.Y);
                        break;
                    }
            }
        }

        private void MainPanel_RButtonDown(MouseEventArgs e)
        {
            int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
            int xc = (e.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot;
            Map.AddPreset(xc, yc);
        }

        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Map.MouseInfo.state != Map.MouseState.click)
            {
                int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
                int xc = (int)((e.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((e.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point pcur = new System.Drawing.Point(xc, yc);
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
                                System.Drawing.Point s = (pcur.X <= Map.MouseInfo.previous.X) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.Y - s.Y) / ((double)t.X - s.X);
                                for (int i = s.X; i <= t.X; i++)
                                    Map.Change(i, (int)(s.Y + ((double)i - s.X) * k), 1);
                            }
                            {
                                System.Drawing.Point s = (pcur.Y <= Map.MouseInfo.previous.Y) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.X - s.X) / ((double)t.Y - s.Y);
                                for (int i = s.Y; i <= t.Y; i++)
                                    Map.Change((int)(s.X + ((double)i - s.Y) * k), i, 1);
                            }
                            Map.MouseInfo.previous = pcur;
                            break;
                        }
                    case Map.MouseState.eraser:
                        {
                            {
                                System.Drawing.Point s = (pcur.X <= Map.MouseInfo.previous.X) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.Y - s.Y) / ((double)t.X - s.X);
                                for (int i = s.X; i <= t.X; i++)
                                    Map.Change(i, (int)(s.Y + ((double)i - s.X) * k), 2);
                            }
                            {
                                System.Drawing.Point s = (pcur.Y <= Map.MouseInfo.previous.Y) ? pcur : Map.MouseInfo.previous, t = (s == pcur) ? Map.MouseInfo.previous : pcur;
                                double k = ((double)t.X - s.X) / ((double)t.Y - s.Y);
                                for (int i = s.Y; i <= t.Y; i++)
                                    Map.Change((int)(s.X + ((double)i - s.Y) * k), i, 2);
                            }
                            Map.MouseInfo.previous = pcur;
                            break;
                        }
                    case Map.MouseState.select:
                        {
                            Map.MouseInfo.select_second = new System.Drawing.Point(e.X, e.Y);
                            break;
                        }
                }
            }
        }

        private void MainPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (Map.AddRgnInfo.state != Map.AddRegionState.normal)
            {
                int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p1 = new System.Drawing.Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p2 = new System.Drawing.Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new System.Drawing.Point();
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
        }

        private void MainPanel_MouseWheel(object sender, MouseEventArgs e)
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
            const int move_length = 40;
            if (e.Control || e.Alt || e.Shift) { e.Handled = false; return; }
            switch (e.KeyCode)
            {
                case Keys.B:
                    {
                        Map.KeybdInputState = Map.KeyboardInputState.preset;
                        Program.SetMainLabel("Press a key to select preset...");
                        break;
                    }
                case Keys.D:
                    {
                        Map.KeybdInputState = Map.KeyboardInputState.direction;
                        Program.SetMainLabel("Press a key to select direction...");
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
                        Program.SetMainLabel("Map reset", 1500);
                        break;
                    }
                case Keys.Left:
                    {
                        Program.control.XPivot.Value -= move_length / Map.Scale + 1;
                        Program.SetMainLabel("X pivot: " + Map.XPivot, 500);
                        break;
                    }
                case Keys.Right:
                    {
                        Program.control.XPivot.Value += move_length / Map.Scale + 1;
                        Program.SetMainLabel("X pivot: " + Map.XPivot, 500);
                        break;
                    }
                case Keys.Up:
                    {
                        Program.control.YPivot.Value -= move_length / Map.Scale + 1;
                        Program.SetMainLabel("Y pivot: " + Map.YPivot, 500);
                        break;
                    }
                case Keys.Down:
                    {
                        Program.control.YPivot.Value += move_length / Map.Scale + 1;
                        Program.SetMainLabel("Y pivot: " + Map.YPivot, 500);
                        break;
                    }
                case Keys.Oemplus:
                    {
                        Program.control.Timer.Value = Program.control.Timer.Value - 5 >= Program.control.Timer.Minimum ? Program.control.Timer.Value - 5 : Program.control.Timer.Minimum;
                        break;
                    }
                case Keys.OemMinus:
                    {
                        Program.control.Timer.Value = Program.control.Timer.Value + 5 <= Program.control.Timer.Maximum ? Program.control.Timer.Value + 5 : Program.control.Timer.Maximum;
                        break;
                    }
                default:
                    {
                        if (!(e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)) { e.Handled = false; return; }

                        switch (Map.KeybdInputState)
                        {
                            case Map.KeyboardInputState.normal: { e.Handled = false; return; }
                            case Map.KeyboardInputState.preset:
                                {
                                    try { Program.control.PresetSelect.Value = e.KeyCode - Keys.D0; }
                                    catch (ArgumentOutOfRangeException) { Program.SetMainLabel("Out of range", 1000); }
                                    break;
                                }
                            case Map.KeyboardInputState.direction:
                                {
                                    try { Program.control.DirectionSelect.Value = e.KeyCode - Keys.D0; }
                                    catch (ArgumentOutOfRangeException) { Program.SetMainLabel("Out of range", 1000); }
                                    break;
                                }
                        }
                        Map.KeybdInputState = Map.KeyboardInputState.normal;
                        break;
                    }
            }
            e.Handled = true;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e) { }

        private void EditCreateRandom_Click(object sender, EventArgs e)
        {
            Map.AddRgnInfo.state = Map.AddRegionState.random;
            if (Map.MouseInfo.select_first.IsEmpty)
            {
                Map.AddRgnInfo.lastMouseState = Map.MouseInfo.state;
                Program.control.MouseStateSelect.Checked = true;
                Program.SetMainLabel("Create a random rectangle: Drag to select the rectangle");
            }
            else
            {
                int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p1 = new System.Drawing.Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p2 = new System.Drawing.Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new System.Drawing.Point();
                Map.AddRgnInfo.state = Map.AddRegionState.normal;
                Program.SetMainLabel("Random rectangle created", 1000);
            }
        }

        private void EditCreateSolid_Click(object sender, EventArgs e)
        {
            Map.AddRgnInfo.state = Map.AddRegionState.insert;
            if (Map.MouseInfo.select_first.IsEmpty)
            {
                Map.AddRgnInfo.lastMouseState = Map.MouseInfo.state;
                Program.control.MouseStateSelect.Checked = true;
                Program.SetMainLabel("Create a solid rectangle: Drag to select the rectangle");
            }
            else
            {
                int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p1 = new System.Drawing.Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p2 = new System.Drawing.Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new System.Drawing.Point();
                Map.AddRgnInfo.state = Map.AddRegionState.normal;
                Program.SetMainLabel("Solid rectangle created", 1000);
            }
        }

        private void EditDeleteRegion_Click(object sender, EventArgs e)
        {
            Map.AddRgnInfo.state = Map.AddRegionState.delete;
            if (Map.MouseInfo.select_first.IsEmpty)
            {
                Map.AddRgnInfo.lastMouseState = Map.MouseInfo.state;
                Program.control.MouseStateSelect.Checked = true;
                Program.SetMainLabel("Delete a rectangle: Drag to select the rectangle");
            }
            else
            {
                int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
                int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p1 = new System.Drawing.Point(xc, yc);
                xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
                yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
                System.Drawing.Point p2 = new System.Drawing.Point(xc, yc);
                Map.AddDeleteRegion(p1, p2);
                Map.MouseInfo.select_first = Map.MouseInfo.select_second = new System.Drawing.Point();
                Map.AddRgnInfo.state = Map.AddRegionState.normal;
                Program.SetMainLabel("Rectangle deleted", 1000);
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
                Program.SetMainLabel("Loading file " + fname);
                try
                {
                    if (fname.EndsWith(".lfs")) Map.LoadLFS(fname);
                    if (fname.EndsWith(".lf")) Map.LoadLF(fname);
                    Program.SetMainLabel("File loaded", 1000);
                } 
                catch (Exception exception)
                {
                    Program.control.Reset_Click(null, null);
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK);
                    Program.SetMainLabel("Load failed", 1000);
                }
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
            if (Map.MouseInfo.select_first.IsEmpty && Map.MouseInfo.select_second.IsEmpty) return;
            int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
            int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
            int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
            Map.CopyInfo.first = new System.Drawing.Point(xc, yc);
            xc = (int)((Map.MouseInfo.select_second.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
            yc = (int)((Map.MouseInfo.select_second.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
            Map.CopyInfo.second = new System.Drawing.Point(xc, yc);
            Map.CopyInfo.state = true;
            Map.MouseInfo.select_first = Map.MouseInfo.select_second = new System.Drawing.Point();
            Program.SetMainLabel("Select a cell and paste");
        }

        private void EditPaste_Click(object sender, EventArgs e)
        {
            if (!Map.CopyInfo.state) return;
            if (Map.MouseInfo.select_first != Map.MouseInfo.select_second) return; 
            int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
            int xc = (int)((Map.MouseInfo.select_first.X - mid_x + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.XPivot);
            int yc = (int)((Map.MouseInfo.select_first.Y - mid_y + 0x1000 * Map.Scale) / Map.Scale - 0x1000 + Map.YPivot);
            Map.Paste(xc, yc);
            Map.CopyInfo.state = false;
            Program.SetMainLabel("Pasted", 1000);
        }

        private void MainPanel_SizeChanged(object sender, EventArgs e)
        {
            PaintTools.renderTarget?.Resize(new Size2(Program.main.MainPanel.Width, Program.main.MainPanel.Height));
        }

        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            MainPanel_Paint();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            PaintTimer.Start();
            Program.SetMainLabel("Initiallized", 2000);
        }
    }
}
