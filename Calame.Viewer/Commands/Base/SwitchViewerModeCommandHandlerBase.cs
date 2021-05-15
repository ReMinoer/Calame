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

        protected override bool CanRun(IViewerDocument document)
        {
            return document?.Viewer?.InteractiveModes.AnyOfType<TMode>() ?? false;
        }

        protected override void UpdateStatus(Command command, IViewerDocument document)
        {
            base.UpdateStatus(command, document);
            command.Checked = document?.Viewer?.SelectedMode is TMode;
        }

        protected override void Run(IViewerDocument document)
        {
            var interactiveMode = document.Viewer.InteractiveModes.FirstOfType<TMode>();
            _eventAggregator.PublishAsync(new SwitchViewerModeRequest(document, interactiveMode)).Wait();
        }
    }
}