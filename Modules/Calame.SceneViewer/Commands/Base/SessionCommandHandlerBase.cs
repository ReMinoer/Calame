using System;
using System.Threading.Tasks;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands.Base
{
    public abstract class SessionCommandHandlerBase<TSession, TCommandDefinition> : SceneViewerCommandHandlerBase<TCommandDefinition>
        where TSession : class, ISession
        where TCommandDefinition : CommandDefinition
    {
        private TSession _session;

        protected override void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            base.OnActiveDocumentChanged(sender, e);
            _session = Document?.Session as TSession;
        }

        protected override sealed void RefreshContext(Command command, SceneViewerViewModel document)
        {
            base.RefreshContext(command, document);

            if (_session == null && Document != null)
                _session = Document.Session as TSession;

            RefreshContext(command, _session);
        }

        protected override sealed bool CanRun(Command command, SceneViewerViewModel document)
        {
            return base.CanRun(command, document)
                && _session != null
                && CanRun(command, _session);
        }

        protected override sealed void UpdateStatus(Command command, SceneViewerViewModel document)
        {
            base.UpdateStatus(command, document);
            UpdateStatus(command, _session);
        }

        protected override sealed Task RunAsync(Command command, SceneViewerViewModel document)
        {
            return RunAsync(command, _session);
        }

        protected virtual void RefreshContext(Command command, TSession session) {}
        protected virtual bool CanRun(Command command, TSession session) => true;
        protected virtual void UpdateStatus(Command command, TSession session) { }
        protected abstract Task RunAsync(Command command, TSession session);
    }
}