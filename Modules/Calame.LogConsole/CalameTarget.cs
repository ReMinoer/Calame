using System;
using NLog;
using NLog.Targets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Calame.LogConsole
{
    [Target(nameof(Calame))]
    public class CalameTarget : Target
    {
        public event EventHandler<LogEntry> MessageLogged;

        protected override void Write(LogEventInfo logEvent)
        {
            MessageLogged?.Invoke(this, new LogEntry
            {
                TimeStamp = logEvent.TimeStamp,
                Source = logEvent.LoggerName,
                Level = ConvertLogLevel(logEvent.Level),
                Message = logEvent.FormattedMessage,
            });
        }

        private LogLevel ConvertLogLevel(NLog.LogLevel level)
        {
            if (level == NLog.LogLevel.Trace)
                return LogLevel.Trace;
            if (level == NLog.LogLevel.Debug)
                return LogLevel.Debug;
            if (level == NLog.LogLevel.Info)
                return LogLevel.Information;
            if (level == NLog.LogLevel.Warn)
                return LogLevel.Warning;
            if (level == NLog.LogLevel.Error)
                return LogLevel.Error;
            if (level == NLog.LogLevel.Fatal)
                return LogLevel.Critical;
            if (level == NLog.LogLevel.Off)
                return LogLevel.None;

            return LogLevel.None;
        }
    }
}