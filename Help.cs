using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
            HelpContent.Text = "space \t\t- start/stop\r\n" +
                "leftclick \t\t - select / unselect(Click) / draw(Pen) / erase(Eraser)\r\n" +
                "rightclick \t\t - deploy builtin\r\n" +
                "ctrl + tab \t\t - switch focus(if control window is shown)\r\n" +
                "b, 0 - 5 \t\t - select builtin\r\n" +
                "d, 0 - 7 \t\t - select direction\r\n" +
                "c \t\t - Show / hide Control Dialog\r\n" +
                "= (+) \t\t - faster\r\n" +
                "-(_) \t\t - slower\r\n" +
                "ctrl + o \t\t - open file\r\n" +
                "ctrl + s \t\t - save file\r\n" +
                "ctrl + h \t\t - help(this dialog)\r\n" +
                "Options->C / D \t - Click two points to create/ delete a rectangle region\r\n" +
                "Find more interesting seeds in ./ seeds";
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://baike.baidu.com/item/%E5%BA%B7%E5%A8%81%E7%94%9F%E5%91%BD%E6%B8%B8%E6%88%8F/22668799");
        }
    }
}
