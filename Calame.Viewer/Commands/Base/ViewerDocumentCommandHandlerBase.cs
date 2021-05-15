using Calame.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.Viewer.Commands.Base
{
    public abstract class ViewerDocumentCommandHandlerBase<TViewerDocument, TCommandDefinition> : DocumentCommandHandlerBase<TViewerDocument, TCommandDefinition>
        where TViewerDocument : class, IViewerDocument
        where TCommandDefinition : CommandDefinition
    {
        protected override bool CanRun(TViewerDocument document)
        {
            return base.CanRun(document)
                && (document.Viewer.Runner?.Engine?.IsStarted ?? false);
        }
    }
}