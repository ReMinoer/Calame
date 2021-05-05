using System;
using Caliburn.Micro;
using Microsoft.Extensions.Logging;

namespace Calame.LogConsole
{
    public class LogEntry : PropertyChangedBase
    {
        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get => _timeStamp;
            set => Set(ref _timeStamp, value);
        }

        private string _source;
        public string Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        private LogLevel _level;
        public LogLevel Level
        {
            get => _level;
            set => Set(ref _level, value);
        }

        private string _category;
        public string Category
        {
            get => _category;
            set => Set(ref _category, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        private bool _visibleForFilter = true;
        public bool VisibleForFilter
        {
            get => _visibleForFilter;
            set => Set(ref _visibleForFilter, value);
        }
    }
}