using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Calame.LogConsole.ViewModels
{
    [Export(typeof(LogConsoleViewModel))]
    public sealed class LogConsoleViewModel : CalameTool<IDocumentContext>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Bottom;
        protected override object IconKey => CalameIconKey.LogConsole;

        [ImportingConstructor]
        public LogConsoleViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Log Console";
        }

        protected override Task OnDocumentActivated(IDocumentContext activeDocument) => Task.CompletedTask;
        protected override Task OnDocumentsCleaned() => Task.CompletedTask;
    }
}