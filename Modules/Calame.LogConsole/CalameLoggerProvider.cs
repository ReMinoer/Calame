using System.ComponentModel.Composition;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Calame.LogConsole
{
    [Export(typeof(ILoggerProvider))]
    public class CalameLoggerProvider : ILoggerProvider
    {
        private readonly NLogLoggerProvider _loggerProvider;

        public CalameLoggerProvider()
        {
            _loggerProvider = new NLogLoggerProvider();
        }

        public ILogger CreateLogger(string categoryName) => _loggerProvider.CreateLogger(categoryName);
        public void Dispose() => _loggerProvider.Dispose();
    }
}