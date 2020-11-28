using Calame.DataModelViewer.ViewModels;
using Calame.Viewer.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.DataModelViewer.Commands.Base
{
    public abstract class DataModelViewerCommandHandlerBase<TCommandDefinition> : ViewerDocumentCommandHandlerBase<DataModelViewerViewModel, TCommandDefinition>
        where TCommandDefinition : CommandDefinition
    {
    }
}