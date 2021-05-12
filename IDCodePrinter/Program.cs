using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
namespace IDCodePrinter
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("程序已启动，请勿重复打开");
                    System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                }
            }
        }
    }
}
