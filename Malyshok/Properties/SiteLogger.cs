using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Portal.Code
{
    internal static class AppLogger
    {
        private static Logger mLogger = LogManager.GetCurrentClassLogger();

        public static void Trace(string message, Exception exception = null, Dictionary<string, object> extParams = null)
        {
            mLogger.Trace(exception, MakeLogString(message, exception, extParams));
        }

        public static void Debug(string message, Exception exception = null,  Dictionary<string, object> extParams = null)
        {
            mLogger.Debug(exception, MakeLogString(message, exception, extParams));
        }

        public static void Info(String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Info(exception, MakeLogString(message, exception, extParams));
        }

        public static void Warn(String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Warn(exception, MakeLogString(message, exception, extParams));
        }

        public static void Error(String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Error(exception, MakeLogString(message, exception,  extParams));
        }

        public static void Fatal(String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Fatal(exception, MakeLogString(message, exception, extParams));
        }

        private static string MakeLogString(string message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine(message == null ? "<no log message>" : message);

            if (extParams != null)
            {
                var extParamsString = "<none>";
                if (extParams != null && extParams.Any())
                    extParamsString = String.Join(";", extParams.Select(p => String.Format("{0}={1}", String.IsNullOrEmpty(p.Key) ? "<unknown>" : p.Key, p.Value == null ? "<null>" : p.Value)));
                log
                    .AppendFormat("ExtParams: {0}", extParamsString)
                    .AppendLine();
            }

            if (exception != null)
            {
                log.AppendLine();
                log.AppendLine("Exception:");
                log.AppendLine(exception.ToString());
                log.AppendLine(exception.Message);
                if (exception.InnerException != null)
                {
                    log.AppendLine(exception.InnerException.Message);
                }
                if (exception.StackTrace != null)
                {
                    log.AppendLine();
                    log.AppendLine("StackTrace:");
                    log.AppendLine(exception.StackTrace);
                }
            }

            return log.ToString();
        }
    }
}