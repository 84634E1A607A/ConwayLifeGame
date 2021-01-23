using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    public partial class Control : Form
    {
        readonly private Bitmap previewBitmap;
        readonly Graphics graphics;
        readonly private SolidBrush brush;
        readonly private SolidBrush rbrush;
        private Size size;

        public Control()
        {
            InitializeComponent();
            size = PreviewPictureBox.Size;
            previewBitmap = new Bitmap(size.Width, size.Height);
            brush = new SolidBrush(Color.Black);
            rbrush = new SolidBrush(Color.FromArgb(0x80, Color.Red));
            graphics = Graphics.FromImage(previewBitmap);
            PresetSelect.Maximum = Map.PresetNum;
            PreviewPictureBox_Paint();
            MouseStateClick.Checked = true;
        }

        private void PreviewPictureBox_SizeChanged(object sender, EventArgs e)
        {
            size = PreviewPictureBox.Size;
        }

        private void PreviewPictureBox_Paint()
        {
            Map.Preset preset_info = Map.GetBulitinInfo();
            graphics.Clear(BackColor);
            int scale, xStart, yStart;
            if (Map.SelectedDirection <= 4)
            {
                scale = Math.Min(size.Width / preset_info.width, size.Height / preset_info.height);
                xStart = (size.Width - preset_info.width * scale) / 2; yStart = (size.Height - preset_info.height * scale) / 2;
            }
            else
            {
                scale = Math.Min(size.Width / preset_info.height, size.Height / preset_info.width);
                xStart = (size.Width - preset_info.height * scale) / 2; yStart = (size.Height - preset_info.width * scale) / 2;
            }
            graphics.TranslateTransform(xStart, yStart);
            switch (Map.SelectedDirection)
            {
                case 1:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * point.X, scale * point.Y, scale, scale));
                    break;
                case 2:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * point.X, scale * (preset_info.height - 1 - point.Y), scale, scale));
                    break;
                case 3:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * (preset_info.width - 1 - point.X), scale * point.Y, scale, scale));
                    break;
                case 4:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * (preset_info.width - 1 - point.X), scale * (preset_info.height - 1 - point.Y), scale, scale));
                    break;
                case 5:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * point.Y, scale * point.X, scale, scale));
                    break;
                case 6:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * point.Y, scale * (preset_info.width - 1 - point.X), scale, scale));
                    break;
                case 7:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * (preset_info.height - 1 - point.Y), scale * point.X, scale, scale));
                    break;
                case 8:
                    foreach (Point point in preset_info.points)
                        graphics.FillRectangle(brush, new Rectangle(scale * (preset_info.height - 1 - point.Y), scale * (preset_info.width - 1 - point.X), scale, scale));
                    break;
            }
            graphics.FillRectangle(rbrush, new Rectangle((int)(scale * 0.1), (int)(scale * 0.1), (int)(scale * 0.8), (int)(scale * 0.8)));
            graphics.ResetTransform();
            PreviewPictureBox.Image = previewBitmap;
        }

        public void StartStop_Click(object sender, EventArgs e)
        {
            if (Map.MouseInfo.state == Map.MouseState.select && Map.Started == false) return;
            Map.Started = !Map.Started;
            if (Map.Started)
            {
                Program.main.ClacTimer.Start();
                Program.main.StatusLabel.Text = "Running";
            }
            else {
                Program.main.ClacTimer.Stop();
                Program.main.StatusLabel.Text = "Paused";
            }
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
            Program.SetMainLabel("Map reset", 1500);
        }

        private void PresetSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.SelectedPreset = (int)PresetSelect.Value;
            PreviewPictureBox_Paint();
            Program.SetMainLabel("Selected preset: " + Map.SelectedPreset, 1000);
        }

        private void DirectionSelect_ValueChanged(object sender, EventArgs e)
        {
            Map.SelectedDirection = (int)DirectionSelect.Value;
            PreviewPictureBox_Paint();
            Program.SetMainLabel("Selected direction: " + Map.SelectedDirection, 1000);
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
            Program.SetMainLabel("Timer: " + Map.Timer, 500);
        }

        private void MouseState_CheckedChanged(object sender, EventArgs e)
        {
            if (MouseStateClick.Checked) Map.MouseInfo.state = Map.MouseState.click;
            else if (MouseStatePen.Checked) Map.MouseInfo.state = Map.MouseState.pen;
            else if (MouseStateEraser.Checked) Map.MouseInfo.state = Map.MouseState.eraser;
            else if (MouseStateDrag.Checked) Map.MouseInfo.state = Map.MouseState.drag;
            else if (MouseStateSelect.Checked) { Map.MouseInfo.state = Map.MouseState.select; StartStop_Click(null, null); }
            if (!MouseStateSelect.Checked) Map.MouseInfo.select_first = Map.MouseInfo.select_second = new Point();
            Program.SetMainLabel("Mouse function: " + Map.MouseInfo.state.ToString(), 1000);
        }
    }
}
