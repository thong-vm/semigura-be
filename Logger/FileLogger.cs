using System.Diagnostics;

namespace Logger
{
    public class FileLogger : ILogger
    {
        //protected readonly RoundTheCodeFileLoggerProvider _roundTheCodeLoggerFileProvider;
        private readonly string _name;
        private readonly Func<FileLoggerConfig> _getCurrentConfig;

        //public RoundTheCodeFileLogger([NotNull] RoundTheCodeFileLoggerProvider roundTheCodeLoggerFileProvider)
        //{
        //    _roundTheCodeLoggerFileProvider = roundTheCodeLoggerFileProvider;
        //}
        public FileLogger(string name, Func<FileLoggerConfig> getCurrentConfig)
        {
            _name = name;
            _getCurrentConfig = getCurrentConfig;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            try
            {

                var options = _getCurrentConfig();
                var fullFilePath = options.FolderPath + "/" + options.FilePath.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd"));
                var logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]", logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "");

                using (var streamWriter = new StreamWriter(fullFilePath, true))
                {
                    streamWriter.WriteLine(logRecord);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
