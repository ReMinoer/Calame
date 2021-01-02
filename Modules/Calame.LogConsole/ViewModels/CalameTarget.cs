using System;
using NLog;
using NLog.Targets;

namespace Calame.LogConsole.ViewModels
{
    [Target(nameof(Calame))]
    public class CalameTarget : Target
    {
        public event EventHandler<LogEventInfo> MessageLogged;

        protected override void Write(LogEventInfo logEvent)
        {
            MessageLogged?.Invoke(this, logEvent);
        }
    }
}