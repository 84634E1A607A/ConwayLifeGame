using System;
using System.Collections.Generic;
using System.Text;

namespace ConwayLifeGame
{
    public static class AutoScale
    {
        static System.Windows.Forms.Control control;
        static System.Drawing.Graphics g;
        public static float GetScale(System.Windows.Forms.Control c)
        {
            if (c != control) { control = c; g = control?.CreateGraphics(); }
            float d = g?.DpiX ?? 0;
            return  d / 96.0f;
        }
    }
}
