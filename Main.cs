using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void OptionsShowWindow_Click(object sender, EventArgs e)
        {
            Program.control.Show();
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Graphics graphics;
        private Pen pen;
        private Bitmap bitmap;
        private int bitmapScale;

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            if (graphics == null) graphics = MainPanel.CreateGraphics();

            Size size = MainPanel.Size;
            int mid_x = size.Width / 2, mid_y = size.Height / 2;

            if (pen == null) pen = new Pen(Color.FromArgb(0xFF, 0x88, 0x88, 0x88), 1);

            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            BufferedGraphics bufferedGraphics = context.Allocate(graphics, e.ClipRectangle);
            Graphics buffered = bufferedGraphics.Graphics;

            buffered.Clear(BackColor);

            if (bitmap == null || bitmap.Size != size || bitmapScale != Map.scale)
            {
                if (bitmap != null) bitmap.Dispose();
                bitmap = new Bitmap(size.Width, size.Height);
                Graphics bitmapGraphics = Graphics.FromImage(bitmap);
                /*  lines in bitmap */
                for (int i = mid_x % Map.scale; i <= size.Width; i += Map.scale)
                    bitmapGraphics.DrawLine(pen, new Point(i, 0), new Point(i, size.Height));
                for (int i = mid_y % Map.scale; i <= size.Height; i += Map.scale)
                    bitmapGraphics.DrawLine(pen, new Point(0, i), new Point(size.Width, i));
                bitmapGraphics.Dispose();
                bitmapScale = Map.scale;
            }

            /*  lines   */
            buffered.DrawImage(bitmap, 0, 0);
            /*  blocks  */
            Map.Draw(buffered, size);

            bufferedGraphics.Render();
            bufferedGraphics.Dispose();
        }

        private void MainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) MainPanel_LButtonDown(e);
            else if (e.Button == MouseButtons.Right) MainPanel_RButtonDown(e);

        }

        private void MainPanel_LButtonDown(MouseEventArgs e)
        {
            if (Map.add_region_info.state != Map.AddRegionState.normal) return;
            int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
            int xc = (e.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot;
            Map.Change(xc, yc);
            if (Map.add_region_info.state != Map.AddRegionState.normal)
            {
                if (!Map.add_region_info.count)
                {
                    Map.add_region_info.point = new Point();
                    Map.add_region_info.point.X = xc;
                    Map.add_region_info.point.Y = yc;
                    Map.add_region_info.count = true;
                }
                else
                {
                    Point p1 = new Point(), p2 = new Point();
                    p1.X = Math.Min(Map.add_region_info.point.X, xc);
                    p1.Y = Math.Min(Map.add_region_info.point.Y, yc);
                    p2.X = Math.Max(Map.add_region_info.point.X, xc);
                    p2.Y = Math.Max(Map.add_region_info.point.Y, yc);
                    Map.AddDeleteRegion(p1, p2);
                    Map.add_region_info.count = false;
                    Map.add_region_info.state = Map.AddRegionState.normal;
                }
            }
            else
            {
                Map.add_region_info.point = new Point(xc, yc);
            }
            Program.main.MainPanel.Refresh();
        }

        private void MainPanel_RButtonDown(MouseEventArgs e)
        {
            int mid_x = MainPanel.Width / 2, mid_y = MainPanel.Height / 2;
            int xc = (e.X - mid_x + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.x_pivot;
            int yc = (e.Y - mid_y + 0x1000 * Map.scale) / Map.scale - 0x1000 + Map.y_pivot;
            Map.AddBuiltin(xc, yc);
            MainPanel.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Map.Calc();
            Program.main.MainPanel.Refresh();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            MainPanel.Refresh();
        }
    }
}
