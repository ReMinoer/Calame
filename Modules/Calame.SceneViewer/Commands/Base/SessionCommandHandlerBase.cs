using System.Threading.Tasks;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands.Base
{
    public abstract class SessionCommandHandlerBase<TSession, TCommandDefinition> : SceneViewerCommandHandlerBase<TCommandDefinition>
        where TSession : class, ISession
        where TCommandDefinition : CommandDefinition
    {
        protected override sealed bool CanRun(Command command, SceneViewerViewModel document)
        {
            return base.CanRun(command, document)
                && document.Session is TSession session
                && CanRun(command, session);
        }

        protected override sealed Task RunAsync(Command command, SceneViewerViewModel document)
        {
            if (document.Session is TSession activeSession)
                return RunAsync(command, activeSession);

            return Task.CompletedTask;
        }

        protected virtual bool CanRun(Command command, TSession session) => true;
        protected abstract Task RunAsync(Command command, TSession session);
    }
}