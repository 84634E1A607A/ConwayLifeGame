using System;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.Text = "About";
        }

        private void About_Load(object sender, EventArgs e)
        {

        }

        private void OK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
