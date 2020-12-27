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
    public partial class CopyConflict : Form
    {
        public CopyConflict()
        {
            InitializeComponent();
        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            Map.copy_info.copyState = Map.CopyState.replace;
            Close();
        }

        private void MergeButton_Click(object sender, EventArgs e)
        {
            Map.copy_info.copyState = Map.CopyState.merge;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Map.copy_info.copyState = Map.CopyState.cancel;
            Close();
        }
    }
}
