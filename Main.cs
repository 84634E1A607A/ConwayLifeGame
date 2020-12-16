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
            Map.InitBuiltins();
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

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            if (graphics == null) graphics = MainPanel.CreateGraphics();

            Size size = MainPanel.Size;
            int mid_x = size.Width / 2, mid_y = size.Height / 2;

            if (pen == null) pen = new Pen(Color.Black, 1);

            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            BufferedGraphics bufferedGraphics = context.Allocate(graphics, e.ClipRectangle);
            Graphics buffered = bufferedGraphics.Graphics;

            buffered.Clear(BackColor);

            /*  lines  */
            for (int i = mid_x % Map.scale; i <= size.Width; i += Map.scale)
                buffered.DrawLine(pen, new Point(i, 0), new Point(i, size.Height));
            for (int i = mid_y % Map.scale; i <= size.Height; i += Map.scale)
                buffered.DrawLine(pen, new Point(0, i), new Point(size.Width, i));

            /*  blocks  */
            Map.Draw(buffered, size);

            bufferedGraphics.Render();
            bufferedGraphics.Dispose();

        }
    }
}
