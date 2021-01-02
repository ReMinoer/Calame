using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework.Services;
using NLog;
using LogManager = NLog.LogManager;

namespace Calame.LogConsole.ViewModels
{
    [Export(typeof(LogConsoleViewModel))]
    public sealed class LogConsoleViewModel : CalameTool<IDocumentContext>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Bottom;
        protected override object IconKey => CalameIconKey.LogConsole;

        public ObservableCollection<LogEventInfo> LogEntries { get; }

        [ImportingConstructor]
        public LogConsoleViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Log Console";
            LogEntries = new ObservableCollection<LogEventInfo>();

            CalameTarget logTarget = LogManager.Configuration.AllTargets.OfType<CalameTarget>().FirstOrDefault();
            if (logTarget != null)
                logTarget.MessageLogged += OnMessageLogged;
        }

        private void OnMessageLogged(object sender, LogEventInfo e)
        {
            Application.Current.Dispatcher.InvokeAsync(() => LogEntries.Add(e));
        }

        protected override Task OnDocumentActivated(IDocumentContext activeDocument) => Task.CompletedTask;
        protected override Task OnDocumentsCleaned() => Task.CompletedTask;
    }
}