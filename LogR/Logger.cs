using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LogR
{
    public class Logger
    {
        public static log4net.ILog logger;

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void Debug(Exception ex, string msg = "", Type t = null)
        {
            //using System.Reflection;
            //MethodBase.GetCurrentMethod().DeclaringType
            //获取当前文件名
            if (t != null)
                logger = log4net.LogManager.GetLogger(t);
            else
                logger = log4net.LogManager.GetLogger("");
            logger.Debug(msg, ex);
        }

        public static void Error(Exception ex, string msg = "", Type t = null)
        {
            if (t != null)
                logger = log4net.LogManager.GetLogger(t);
            else
                logger = log4net.LogManager.GetLogger("");
            logger.Error(msg, ex);
        }

        public static void Fatal(Exception ex, string msg = "", Type t = null)
        {
            if (t != null)
                logger = log4net.LogManager.GetLogger(t);
            else
                logger = log4net.LogManager.GetLogger("");
            logger.Fatal(msg, ex);
        }

        public static void Info(string msg, Type t = null)
        {
            if (t != null)
                logger = log4net.LogManager.GetLogger(t);
            else
                logger = log4net.LogManager.GetLogger("");
            logger.Info(msg);
        }

        public static void Warning(Exception ex, string msg = "", Type t = null)
        {
            if (t != null)
                logger = log4net.LogManager.GetLogger(t);
            else
                logger = log4net.LogManager.GetLogger("");
            logger.Warn(msg, ex);
        }
    }
}
