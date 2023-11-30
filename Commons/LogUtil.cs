namespace semigura.Commons
{
    // stub
    public class ILog
    {
        public void Debug(string message) { }
        public void Debug(string message, Exception e) { }
        public void Info(string message) { }
        public void Info(string message, Exception e) { }
        public void Warn(string message) { }
        public void Warn(string message, Exception e) { }
        public void Error(string message) { }
        public void Error(string message, Exception e) { }
        public void Fatal(string message) { }
        public void Fatal(string message, Exception e) { }

    }

    public class LogManager
    {
        public static ILog GetLogger(string name)
        {
            return new ILog();
        }
    }

    public class LogUtil
    {
        private static readonly LogUtil _instance = new LogUtil();
        protected ILog monitoringLogger;
        protected static ILog debugLogger;

        private LogUtil()
        {
            monitoringLogger = LogManager.GetLogger("MonitoringLogger");
            debugLogger = LogManager.GetLogger("DebugLogger");
        }

        /// <summary>  
        /// Used to log Debug messages in an explicit Debug Logger  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Debug(string message)
        {
            if (Properties.IS_OUTPUT_LOG_DEBUGGER)
            {
                var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
                debugLogger.Debug(systemDate + " - " + message);
                System.Diagnostics.Debug.WriteLine(systemDate + " - " + message);
            }
        }
        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Debug(string message, System.Exception exception)
        {
            if (Properties.IS_OUTPUT_LOG_DEBUGGER)
            {
                var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
                debugLogger.Debug(systemDate + " - " + message, exception);
                System.Diagnostics.Debug.WriteLine(systemDate + " - " + message, exception.StackTrace);
            }
        }
        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Info(string message)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Info(systemDate + " - " + message);
            System.Diagnostics.Debug.WriteLine(systemDate + " - " + message);
        }
        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Info(string message, System.Exception exception)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Info(systemDate + " - " + message, exception);
            System.Diagnostics.Debug.WriteLine(systemDate + " - " + message, exception.StackTrace);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Warn(string message)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Warn(systemDate + " - " + message);
            System.Diagnostics.Debug.WriteLine(systemDate + " - " + message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Warn(string message, System.Exception exception)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Warn(systemDate + " - " + message, exception);
            System.Diagnostics.Debug.WriteLine(systemDate + " - " + message, exception.StackTrace);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Error(string message)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Error(systemDate + " - " + message);
            System.Diagnostics.Debug.WriteLine(systemDate + " - " + message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Error(string message, System.Exception exception)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Error(message, exception);
            System.Diagnostics.Debug.WriteLine(message, exception.StackTrace);
        }
        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Fatal(string message)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Fatal(systemDate + " - " + message);
            System.Diagnostics.Debug.WriteLine(systemDate + " - " + message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Fatal(string message, System.Exception exception)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy-MM-dd HH:mm:ss");
            _instance.monitoringLogger.Fatal(systemDate + " - " + message, exception);
            System.Diagnostics.Debug.WriteLine(systemDate + " - " + message, exception.StackTrace);
        }
    }
}