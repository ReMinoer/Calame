using System.Threading.Tasks;
using Calame.DataModelViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.DataModelViewer.Commands.Base
{
    public abstract class EditorCommandHandlerBase<TSession, TCommandDefinition> : DataModelViewerCommandHandlerBase<TCommandDefinition>
        where TSession : class, IEditor
        where TCommandDefinition : CommandDefinition
    {
        protected override sealed bool CanShow(Command command, DataModelViewerViewModel document)
        {
            return base.CanShow(command, document)
                && document.Editor is TSession session
                && CanShow(command, session);
        }

        protected override sealed bool CanRun(Command command, DataModelViewerViewModel document)
        {
            return base.CanRun(command, document)
                && document.Editor is TSession session
                && CanRun(command, session);
        }

        protected override sealed Task RunAsync(Command command, DataModelViewerViewModel document)
        {
            if (document.Editor is TSession activeSession)
                return RunAsync(command, activeSession);

            return Task.CompletedTask;
        }

        protected virtual bool CanShow(Command command, TSession session) => true;
        protected virtual bool CanRun(Command command, TSession session) => true;
        protected abstract Task RunAsync(Command command, TSession session);
    }
}