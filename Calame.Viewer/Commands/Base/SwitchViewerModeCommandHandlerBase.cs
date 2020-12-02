using System.Threading.Tasks;
using Calame.Viewer.Messages;
using Caliburn.Micro;
using Diese.Collections;
using Gemini.Framework.Commands;

namespace Calame.Viewer.Commands.Base
{
    public abstract class SwitchViewerModeCommandHandlerBase<TMode, TDefinition> : ViewerDocumentCommandHandlerBase<IViewerDocument, TDefinition>
        where TMode : IViewerInteractiveMode
        where TDefinition : CommandDefinition
    {
        private readonly IEventAggregator _eventAggregator;

        public SwitchViewerModeCommandHandlerBase()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }

        protected override void UpdateStatus(Command command)
        {
            command.Checked = (Shell.ActiveItem as IViewerDocument)?.Viewer?.SelectedMode is TMode;
        }

        protected override bool CanRun(Command command, IViewerDocument document)
        {
            return document?.Viewer?.InteractiveModes.AnyOfType<TMode>() ?? false;
        }

        protected override async Task RunAsync(Command command, IViewerDocument document)
        {
            var interactiveMode = document.Viewer.InteractiveModes.FirstOfType<TMode>();
            await _eventAggregator.PublishAsync(new SwitchViewerModeRequest(document, interactiveMode));
        }
    }
}