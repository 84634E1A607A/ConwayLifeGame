using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    public partial class Control : Form
    {
        public Control()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Map.Builtin builtin_info = Map.GetBulitinInfo();
            Graphics graphics = PreviewPanel.CreateGraphics();
            SolidBrush brush = new SolidBrush(Color.Black);
            Size size = PreviewPanel.Size;
            int x_scale = size.Width / builtin_info.width;
            int y_scale = size.Height / builtin_info.height;
            int scale = x_scale < y_scale ? x_scale : y_scale;
            int x_start = (size.Width - builtin_info.width * scale) / 2;
            int y_start = (size.Height - builtin_info.height * scale) / 2;
            graphics.TranslateTransform(x_start, y_start);
            foreach (Point point in builtin_info.points)
            {
                Rectangle r = new Rectangle(scale * point.X, scale * point.Y, scale, scale);
                graphics.FillRectangle(brush, r);
            }
            SolidBrush rbrush = new SolidBrush(Color.Red);
            graphics.FillRectangle(rbrush, new Rectangle(0, 0, scale, scale));
            rbrush.Dispose();
            brush.Dispose();
            graphics.Dispose();
        }

        private void StartStop_Click(object sender, EventArgs e)
        {
            Map.started = !Map.started;
            if (Map.started) Program.main.ClacTimer.Start();
            else Program.main.ClacTimer.Stop();
        }

        private void Control_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.SetVisibleCore(false);
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Map.Reset();
            Program.main.ClacTimer.Stop();
        }

        public void MapReset()
        {
            XPivot.Value = Map.x_pivot;
            YPivot.Value = Map.y_pivot;
            BuiltinSelect.Value = Map.selected_builtin;
            DirectionSelect.Value = Map.selected_direction;
            Timer.Value = Map.timer;
            MapScale.Value = Map.scale;
            Program.main.MainPanel.Invalidate();
        }

        private void BuiltinSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.selected_builtin = (byte)BuiltinSelect.Value;
            PreviewPanel.Invalidate();
        }

        private void DirectionSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.selected_direction = (byte)DirectionSelect.Value;
        }

        private void XPivot_ValueChanged(object sender, EventArgs e)
        {
            Map.x_pivot = (int)XPivot.Value;
            Program.main.MainPanel.Invalidate();
        }

        private void YPivot_ValueChanged(object sender, EventArgs e)
        {
            Map.y_pivot = (int)YPivot.Value;
            Program.main.MainPanel.Invalidate();
        }

        private void MapScale_ValueChanged(object sender, EventArgs e)
        {
            Map.scale = (int)MapScale.Value;
            Program.main.MainPanel.Invalidate();
        }

        private void Timer_ValueChanged(object sender, EventArgs e)
        {
            Map.timer = (int)Timer.Value;
            Program.main.ClacTimer.Interval = Map.timer;
        }
    }
}
