using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    public partial class Control : Form
    {
        private Bitmap previewBitmap;
        Graphics graphics;
        private SolidBrush brush;
        private SolidBrush rbrush;
        private Size size;

        public Control()
        {
            InitializeComponent();
            size = PreviewPictureBox.Size;
            previewBitmap = new Bitmap(size.Width, size.Height);
            brush = new SolidBrush(Color.Black);
            rbrush = new SolidBrush(Color.Red);
            graphics = Graphics.FromImage(previewBitmap);
            PreviewPictureBox_Paint();
            MouseStateClick.Checked = true;
        }

        private void PreviewPictureBox_Paint()
        {
            Map.Builtin builtin_info = Map.GetBulitinInfo();
            graphics.Clear(BackColor);
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
            graphics.FillRectangle(rbrush, new Rectangle(0, 0, scale, scale));
            graphics.ResetTransform();
            PreviewPictureBox.Image = previewBitmap;
        }

        public void StartStop_Click(object sender, EventArgs e)
        {
            Map.started = !Map.started;
            if (Map.started) Program.main.ClacTimer.Start();
            else Program.main.ClacTimer.Stop();
        }

        private void Control_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetVisibleCore(false);
            e.Cancel = true;
        }

        public void Reset_Click(object sender, EventArgs e)
        {
            Program.main.ClacTimer.Stop();
            // Value Reset
            Map.Reset();
            // Control Reset
            XPivot.Value = Map.x_pivot;
            YPivot.Value = Map.y_pivot;
            BuiltinSelect.Value = Map.selected_builtin;
            DirectionSelect.Value = Map.selected_direction;
            Timer.Value = Map.timer;
            MapScale.Value = Map.scale;
        }

        private void BuiltinSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.selected_builtin = (byte)BuiltinSelect.Value;
            PreviewPictureBox_Paint();
        }

        private void DirectionSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.selected_direction = (byte)DirectionSelect.Value;
        }

        private void XPivot_ValueChanged(object sender, EventArgs e)
        {
            Map.x_pivot = (int)XPivot.Value;
        }

        private void YPivot_ValueChanged(object sender, EventArgs e)
        {
            Map.y_pivot = (int)YPivot.Value;
        }

        private void MapScale_ValueChanged(object sender, EventArgs e)
        {
            Map.scale = (int)MapScale.Value;
        }

        private void Timer_ValueChanged(object sender, EventArgs e)
        {
            Map.timer = (int)Timer.Value;
            Program.main.ClacTimer.Interval = Map.timer;
        }

        private void MouseState_CheckedChanged(object sender, EventArgs e)
        {
            if (MouseStateClick.Checked) Map.mouse_info.state = Map.MouseState.click;
            else if (MouseStatePen.Checked) Map.mouse_info.state = Map.MouseState.pen;
            else if (MouseStateEraser.Checked) Map.mouse_info.state = Map.MouseState.eraser;
            else if (MouseStateDrag.Checked) Map.mouse_info.state = Map.MouseState.drag;
            else if (MouseStateSelect.Checked) Map.mouse_info.state = Map.MouseState.select;
            if (!MouseStateSelect.Checked) Map.mouse_info.select_first = Map.mouse_info.select_second = new Point(0, 0);
        }
    }
}
