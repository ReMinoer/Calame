using System;
using Microsoft.Extensions.Logging;

namespace Calame.LogConsole
{
    public struct LogEntry
    {
        public DateTime TimeStamp { get; set; }
        public string Source { get; set; }
        public LogLevel Level { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
    }
}