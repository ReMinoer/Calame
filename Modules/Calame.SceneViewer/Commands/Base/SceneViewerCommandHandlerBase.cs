using Calame.Commands.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands.Base
{
    public abstract class SceneViewerCommandHandlerBase<TCommandDefinition> : DocumentCommandHandlerBase<SceneViewerViewModel, TCommandDefinition>
        where TCommandDefinition : CommandDefinition
    {
        protected override bool CanRun(Command command, SceneViewerViewModel document)
        {
            return base.CanRun(command, document)
                && (document.Viewer.Runner?.Engine?.IsStarted ?? false);
        }
    }
}