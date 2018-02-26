using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Import.Core
{
    /// <summary>
    /// Логирование ошибок
    /// </summary>
    public static class SrvcLogger
    {

        private static Logger mLogger = LogManager.GetCurrentClassLogger();

        public static void Debug(string perifix, string message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Debug(MakeLogString(perifix, message, exception, extParams));
        }

        public static void Info(string perifix, String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Info(MakeLogString(perifix, message, exception, extParams));
        }

        public static void Warn(string perifix, String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Warn(MakeLogString(perifix, message, exception, extParams));
        }

        public static void Error(string perifix, String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Error(MakeLogString(perifix, message, exception, extParams));
        }

        public static void Fatal(string perifix, String message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            mLogger.Fatal(MakeLogString(perifix, message, exception, extParams));
        }

        private static string MakeLogString(string perifix, string message, Exception exception = null, Dictionary<String, Object> extParams = null)
        {
            var extParamsString = "<none>";
            if (extParams != null && extParams.Any())
                extParamsString = String.Join(";", extParams.Select(p => String.Format("{0}={1}", String.IsNullOrEmpty(p.Key) ? "<unknown>" : p.Key, p.Value == null ? "<null>" : p.Value)));

            StringBuilder log = new StringBuilder();

            log.AppendFormat("{0}: {1}", perifix, message);

            if (exception != null)
            {
                log.AppendFormat("Exception: {0}", exception.ToString());
            };

            if (extParams != null)
            {
                log.AppendFormat("ExtParams: {0}", extParamsString.ToString());
            };

            return log.ToString();
        }
    }
}