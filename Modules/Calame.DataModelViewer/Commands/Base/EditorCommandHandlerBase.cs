using System.Threading.Tasks;
using Calame.DataModelViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.DataModelViewer.Commands.Base
{
    public abstract class EditorCommandHandlerBase<TEditor, TCommandDefinition> : DataModelViewerCommandHandlerBase<TCommandDefinition>
        where TEditor : class, IEditor
        where TCommandDefinition : CommandDefinition
    {
        protected override sealed bool CanRun(Command command, DataModelViewerViewModel document)
        {
            return base.CanRun(command, document)
                && document.Editor is TEditor editor
                && CanRun(command, editor);
        }

        protected override sealed Task RunAsync(Command command, DataModelViewerViewModel document)
        {
            if (document.Editor is TEditor editor)
                return RunAsync(command, editor);

            return Task.CompletedTask;
        }

        protected virtual bool CanRun(Command command, TEditor editor) => true;
        protected abstract Task RunAsync(Command command, TEditor editor);
    }
}