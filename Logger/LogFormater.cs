using System.Runtime.CompilerServices;

namespace Logger
{
    public class LogFormater
    {
        private readonly ILogger _logger = null!;
        public LogFormater(ILogger logger)
        {
            _logger = logger;
        }
        public void Error(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? methodName = null,
            [CallerFilePath] string? fileName = null)
        {
            _logger.LogError("[{0}({1}:{2})] {3}", methodName, System.IO.Path.GetFileName(fileName), lineNumber, message);
        }
        public void Error(string message, Exception ex,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? methodName = null,
            [CallerFilePath] string? fileName = null)
        {
            _logger.LogError(string.Format("[{0}({1}:{2})] {3}", methodName, System.IO.Path.GetFileName(fileName), lineNumber, message), ex);
        }
        public void LogInfo(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? methodName = null,
            [CallerFilePath] string? fileName = null)
        {
            _logger.LogInformation("[{0}({1}:{2})] {3}", methodName, System.IO.Path.GetFileName(fileName), lineNumber, message);
        }

        public void Debug(string message, Exception ex,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? methodName = null,
            [CallerFilePath] string? fileName = null)
        {
            _logger.LogDebug(string.Format("[{0}({1}:{2})] {3}", methodName, System.IO.Path.GetFileName(fileName), lineNumber, message), ex);

        }

        internal void Info(string message, Exception ex,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? methodName = null,
            [CallerFilePath] string? fileName = null)
        {
            _logger.LogInformation(string.Format("[{0}({1}:{2})] {3}", methodName, System.IO.Path.GetFileName(fileName), lineNumber, message), ex);
        }

        internal void Info(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? methodName = null,
            [CallerFilePath] string? fileName = null)
        {
            _logger.LogInformation(string.Format("[{0}({1}:{2})] {3}", methodName, System.IO.Path.GetFileName(fileName), lineNumber, message));
        }
    }
}
