using System;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
            HelpContent.Text = "space - start/stop\r\n" +
              "leftclick - select / unselect(Click) / draw(Pen) / erase(Eraser)\r\n" +
              "rightclick - deploy preset\r\n" +
              "b, num - select preset\r\n" +
              "d, num - select direction\r\n" +
              "c - Show / hide Control Dialog\r\n" +
              "=(+) - faster\r\n" +
              "-(_) - slower\r\n" +
              "Edit->C / D - Click two points to create/ delete a rectangle region\r\n" +
              "Find more interesting seeds in ./ seeds";
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://baike.baidu.com/item/%E5%BA%B7%E5%A8%81%E7%94%9F%E5%91%BD%E6%B8%B8%E6%88%8F/22668799");
        }
    }
}
