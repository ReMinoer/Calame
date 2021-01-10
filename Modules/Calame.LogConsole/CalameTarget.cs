using System;
using System.Collections.Generic;
using NLog;
using NLog.Targets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Calame.LogConsole
{
    [Target(nameof(Calame))]
    public class CalameTarget : TargetWithContext
    {
        public event EventHandler<LogEntry> MessageLogged;

        protected override void Write(LogEventInfo logEvent)
        {
            IDictionary<string, object> context = GetContextMdlc(logEvent);
            MessageLogged?.Invoke(this, new LogEntry
            {
                TimeStamp = logEvent.TimeStamp,
                Source = logEvent.LoggerName,
                Level = ConvertLogLevel(logEvent.Level),
                Category = context != null && context.TryGetValue("Category", out object obj) ? obj?.ToString() : null,
                Message = logEvent.FormattedMessage
            });
        }

        static private LogLevel ConvertLogLevel(NLog.LogLevel level)
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