using Calame.SceneViewer.ViewModels;
using Calame.Viewer.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands.Base
{
    public abstract class SceneViewerCommandHandlerBase<TCommandDefinition> : ViewerDocumentCommandHandlerBase<SceneViewerViewModel, TCommandDefinition>
        where TCommandDefinition : CommandDefinition
    {
    }
}