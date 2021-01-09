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
            PresetSelect.Maximum = Map.PresetNum - 1;
            PreviewPictureBox_Paint();
            MouseStateClick.Checked = true;
        }

        private void PreviewPictureBox_Paint()
        {
            Map.Preset preset_info = Map.GetBulitinInfo();
            graphics.Clear(BackColor);
            int xScale = size.Width / preset_info.width;
            int yScale = size.Height / preset_info.height;
            int scale = xScale < yScale ? xScale : yScale;
            int x_start = (size.Width - preset_info.width * scale) / 2;
            int y_start = (size.Height - preset_info.height * scale) / 2;
            graphics.TranslateTransform(x_start, y_start);
            foreach (Point point in preset_info.points)
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
            if (Map.MouseInfo.state == Map.MouseState.select && Map.Started == false) return;
            Map.Started = !Map.Started;
            if (Map.Started) Program.main.ClacTimer.Start();
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
            XPivot.Value = Map.XPivot;
            YPivot.Value = Map.YPivot;
            PresetSelect.Value = Map.SelectedPreset;
            DirectionSelect.Value = Map.SelectedDirection;
            Timer.Value = Map.Timer;
            MapScale.Value = Map.Scale;
        }

        private void PresetSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.SelectedPreset = (byte)PresetSelect.Value;
            PreviewPictureBox_Paint();
        }

        private void DirectionSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.SelectedDirection = (byte)DirectionSelect.Value;
        }

        private void XPivot_ValueChanged(object sender, EventArgs e)
        {
            Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
            Map.XPivot = (int)XPivot.Value;
        }

        private void YPivot_ValueChanged(object sender, EventArgs e)
        {
            Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
            Map.YPivot = (int)YPivot.Value;
        }

        private void MapScale_ValueChanged(object sender, EventArgs e)
        {
            Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
            Map.Scale = (int)MapScale.Value;
        }

        private void Timer_ValueChanged(object sender, EventArgs e)
        {
            Map.Timer = (int)Timer.Value;
            Program.main.ClacTimer.Interval = Map.Timer;
        }

        private void MouseState_CheckedChanged(object sender, EventArgs e)
        {
            if (MouseStateClick.Checked) Map.MouseInfo.state = Map.MouseState.click;
            else if (MouseStatePen.Checked) Map.MouseInfo.state = Map.MouseState.pen;
            else if (MouseStateEraser.Checked) Map.MouseInfo.state = Map.MouseState.eraser;
            else if (MouseStateDrag.Checked) Map.MouseInfo.state = Map.MouseState.drag;
            else if (MouseStateSelect.Checked) { Map.MouseInfo.state = Map.MouseState.select; StartStop_Click(null, null); }
            if (!MouseStateSelect.Checked) Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
        }
    }
}
