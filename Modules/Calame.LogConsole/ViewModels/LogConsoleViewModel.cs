using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Calame.LogConsole.Commands;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Microsoft.Extensions.Logging;
using LogManager = NLog.LogManager;

namespace Calame.LogConsole.ViewModels
{
    [Export(typeof(LogConsoleViewModel))]
    public sealed class LogConsoleViewModel : CalameTool<IDocumentContext>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Bottom;
        protected override object IconKey => CalameIconKey.LogConsole;

        private string[] _filterTextKeywords = Array.Empty<string>();
        static private readonly char[] FilterSeparators = { ' ' };

        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText == value)
                    return;

                _filterText = value;
                _filterTextKeywords = _filterText.Split(FilterSeparators, StringSplitOptions.RemoveEmptyEntries);

                UpdateFilter();
                NotifyOfPropertyChange();
            }
        }

        public ObservableCollection<LogLevel> HiddenLogLevels { get; } = new ObservableCollection<LogLevel>();

        private bool _autoScroll = true;
        public bool AutoScroll
        {
            get => _autoScroll;
            set => SetValue(ref _autoScroll, value);
        }

        private ObservableCollection<LogEntry> _currentDocumentLogEntries;
        public ObservableCollection<LogEntry> CurrentDocumentLogEntries
        {
            get => _currentDocumentLogEntries;
            set => SetValue(ref _currentDocumentLogEntries, value);
        }

        public List<LogEntry> SelectedLogEntries { get; set; } = new List<LogEntry>();

        public ICommand ClearLogCommand { get; }
        public ICommand AutoScrollLogCommand { get; }
        public ICommand ScrollLogToEndCommand { get; }

        public ICommand CopySelectedLogCommand { get; }
        public ICommand CopyAllLogCommand { get; }

        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }
        public IIconDescriptor<LogLevel> LogLevelIconDescriptor { get; }

        [ImportingConstructor]
        public LogConsoleViewModel(IShell shell, IEventAggregator eventAggregator, ICommandService commandService,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Log Console";

            IconProvider = iconProvider;
            IconDescriptor = iconDescriptorManager.GetDescriptor();
            LogLevelIconDescriptor = iconDescriptorManager.GetDescriptor<LogLevel>();

            CalameTarget logTarget = LogManager.Configuration.AllTargets.OfType<CalameTarget>().FirstOrDefault();
            if (logTarget != null)
                logTarget.MessageLogged += OnMessageLogged;

            HiddenLogLevels.CollectionChanged += OnHiddenLogLevelsChanged;

            ClearLogCommand = commandService.GetTargetableCommand<ClearLogCommand>();
            AutoScrollLogCommand = commandService.GetTargetableCommand<AutoScrollLogCommand>();
            ScrollLogToEndCommand = commandService.GetTargetableCommand<ScrollLogToEndCommand>();

            CopySelectedLogCommand = new RelayCommand(OnCopySelectedLog, CanCopySelectedLog);
            CopyAllLogCommand = new RelayCommand(OnCopyAllLog, CanCopyAllLog);
        }

        public bool ScrollToEndRequested { get; private set; }
        public void ScrollToEnd()
        {
            ScrollToEndRequested = false;
            NotifyOfPropertyChange(nameof(ScrollToEndRequested));
            ScrollToEndRequested = true;
            NotifyOfPropertyChange(nameof(ScrollToEndRequested));
            ScrollToEndRequested = false;
            NotifyOfPropertyChange(nameof(ScrollToEndRequested));
        }

        private bool CanCopySelectedLog(object _) => SelectedLogEntries.Count > 0;
        private void OnCopySelectedLog(object _)
        {
            CopyLog(SelectedLogEntries.OrderBy(x => x.TimeStamp));
        }

        private bool CanCopyAllLog(object _) => CurrentDocumentLogEntries != null && CurrentDocumentLogEntries.Count > 0;
        private void OnCopyAllLog(object _)
        {
            CopyLog(CurrentDocumentLogEntries);
        }

        static private void CopyLog(IEnumerable<LogEntry> log)
        {
            var copyBuilder = new StringBuilder();
            foreach (LogEntry logEntry in log)
                copyBuilder.AppendLine($"{logEntry.TimeStamp:dd-MM-yyyy HH:mm:ss.ffff} | [{logEntry.Level}] {logEntry.Message}");

            Clipboard.SetText(copyBuilder.ToString());
        }

        protected override Task OnDocumentActivated(IDocumentContext activeDocument)
        {
            CurrentDocumentLogEntries = GetLog(activeDocument.Document.Id.ToString());
            SelectedLogEntries.Clear();
            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            CurrentDocumentLogEntries = null;
            SelectedLogEntries.Clear();
            return Task.CompletedTask;
        }

        private void OnMessageLogged(object sender, LogEntry logEntry)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                UpdateFilter(logEntry);

                ObservableCollection<LogEntry> log = GetLog(logEntry.Source);
                bool logWasEmpty = log.Count == 0;
                log.Add(logEntry);

                // Force commands to refresh if log was empty
                if (logWasEmpty)
                    CommandManager.InvalidateRequerySuggested();
            });
        }

        private ObservableCollection<LogEntry> GetLog(string source) => _logBySource.GetOrAdd(source, _ => new ObservableCollection<LogEntry>());
        private readonly ConcurrentDictionary<string, ObservableCollection<LogEntry>> _logBySource = new ConcurrentDictionary<string, ObservableCollection<LogEntry>>();

        private void OnHiddenLogLevelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            if (CurrentDocumentLogEntries == null)
                return;

            foreach (LogEntry logEntry in CurrentDocumentLogEntries)
                UpdateFilter(logEntry);
        }

        private void UpdateFilter(LogEntry logEntry)
        {
            logEntry.VisibleForFilter = MatchLogLevelFilter(logEntry.Level) && MatchTextFilter(logEntry);
        }

        private bool MatchTextFilter(LogEntry logEntry)
        {
            return _filterTextKeywords.Length == 0
                || _filterTextKeywords.Any(x => MatchKeyword(logEntry.Message, x) || MatchKeyword(logEntry.Category, x));
        }

        private bool MatchKeyword(string text, string keyword)
        {
            return text != null && text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool MatchLogLevelFilter(LogLevel logLevel)
        {
            return HiddenLogLevels.Count == 0 || !HiddenLogLevels.Contains(logLevel);
        }
    }
}