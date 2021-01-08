using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;
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

        private ObservableCollection<LogEntry> _currentDocumentLogEntries;
        public ObservableCollection<LogEntry> CurrentDocumentLogEntries
        {
            get => _currentDocumentLogEntries;
            set => SetValue(ref _currentDocumentLogEntries, value);
        }

        public List<LogEntry> SelectedLogEntries { get; set; } = new List<LogEntry>();
        public ICommand CopySelectedLogCommand { get; }
        public ICommand CopyAllLogCommand { get; }

        public IIconProvider IconProvider { get; }
        public IIconDescriptor<LogLevel> LogLevelIconDescriptor { get; }

        [ImportingConstructor]
        public LogConsoleViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Log Console";

            IconProvider = iconProvider;
            LogLevelIconDescriptor = iconDescriptorManager.GetDescriptor<LogLevel>();

            CalameTarget logTarget = LogManager.Configuration.AllTargets.OfType<CalameTarget>().FirstOrDefault();
            if (logTarget != null)
                logTarget.MessageLogged += OnMessageLogged;

            CopySelectedLogCommand = new RelayCommand(
                _ => CopyLog(SelectedLogEntries.OrderBy(x => x.TimeStamp)),
                _ => SelectedLogEntries.Count > 0);
            CopyAllLogCommand = new RelayCommand(
                _ => CopyLog(CurrentDocumentLogEntries),
                _ => CurrentDocumentLogEntries != null && CurrentDocumentLogEntries.Count > 0);
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
                GetLog(logEntry.Source).Add(logEntry);

            });
        }

        private ObservableCollection<LogEntry> GetLog(string source) => _logBySource.GetOrAdd(source, _ => new ObservableCollection<LogEntry>());
        private readonly ConcurrentDictionary<string, ObservableCollection<LogEntry>> _logBySource = new ConcurrentDictionary<string, ObservableCollection<LogEntry>>();
    }
}