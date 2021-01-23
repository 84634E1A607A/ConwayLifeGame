using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConwayLifeGame
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        public static Control control;
        public static Main main;

        private static int mainLabelNo = 0; 
        public static void SetMainLabel(string a = null, int millseconds = 0, string b = null)
        {
            Task.Run(() =>
            {
                int n = ++mainLabelNo;
                if (a != null) main.MainLabel.Text = a;
                if (millseconds != 0)
                {
                    Task.Delay(millseconds).Wait();
                    if (n == mainLabelNo) main.MainLabel.Text = b ?? "";
                }
            });
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(main = new Main());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\r\n" + e.StackTrace, "Exception unhandled");
                throw;
            }
        }
    }
}
