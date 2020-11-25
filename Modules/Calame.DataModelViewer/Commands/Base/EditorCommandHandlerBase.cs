using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.DataModelViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.DataModelViewer.Commands.Base
{
    public abstract class EditorCommandHandlerBase<TEditor, TCommandDefinition> : DocumentCommandHandlerBase<DataModelViewerViewModel, TCommandDefinition>
        where TEditor : class, IEditor
        where TCommandDefinition : CommandDefinition
    {
        protected override sealed bool CanRun(Command command, DataModelViewerViewModel document)
        {
            return document.Editor is TEditor editor && CanRun(command, editor);
        }

        protected override sealed Task RunAsync(Command command, DataModelViewerViewModel document)
        {
            if (document.Editor is TEditor activeEditor)
                return RunAsync(command, activeEditor);

            return Task.CompletedTask;
        }

        protected virtual bool CanRun(Command command, TEditor editor) => true;
        protected abstract Task RunAsync(Command command, TEditor editor);
    }
}