using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Logger
{
    [ProviderAlias("RoundTheCodeFile")]
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable? _onChangeToken;
        private FileLoggerConfig _currentConfig = null!;
        private readonly ConcurrentDictionary<string, FileLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        public FileLoggerProvider(IOptionsMonitor<FileLoggerConfig> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);

            if (!Directory.Exists(_currentConfig.FolderPath))
            {
                Directory.CreateDirectory(_currentConfig.FolderPath);
            }
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new FileLogger(name, GetCurrentConfig));

        private FileLoggerConfig GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
