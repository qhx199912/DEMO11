using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BIW.Common.CrashReport;

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
            #region 全局未处理异常捕获
            Application.ThreadException += (sender, args) =>
            {
                SendCrashReport(args.Exception);
                Environment.Exit(0);
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                SendCrashReport((Exception)args.ExceptionObject);
                Environment.Exit(0);
            };
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new IDCodePrinter());

            bool createNew;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.Run(new IDCodePrinter());
                }
                else
                {
                    MessageBox.Show("程序已启动，请勿重复打开");
                    System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                }
            }
        }

        public static void SendCrashReport(Exception exception, string developerMessage = "")
        {
            var reportCrash = new ReportCrash();
            reportCrash.Send(exception);
        }
    }
}
