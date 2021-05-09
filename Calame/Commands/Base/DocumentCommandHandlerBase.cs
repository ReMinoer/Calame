using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.Commands.Base
{
    public abstract class DocumentCommandHandlerBase<TDocument, TCommandDefinition> : CalameCommandHandlerBase<TCommandDefinition>
        where TDocument : class, IDocument
        where TCommandDefinition : CommandDefinition
    {
        protected readonly IShell Shell;
        protected TDocument Document;

        protected DocumentCommandHandlerBase()
        {
            Shell = IoC.Get<IShell>();
            Shell.ActiveDocumentChanged += OnActiveDocumentChanged;
        }

        protected virtual void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            Document = Shell.ActiveItem as TDocument;
        }

        protected override sealed void RefreshContext(Command command)
        {
            base.RefreshContext(command);
            RefreshContext(command, Document);
        }

        protected override sealed bool CanRun(Command command)
        {
            return base.CanRun(command)
                && Document != null
                && CanRun(command, Document);
        }

        protected override sealed void UpdateStatus(Command command)
        {
            base.UpdateStatus(command);
            UpdateStatus(command, Document);
        }

        protected override sealed Task RunAsync(Command command)
        {
            if (Document != null)
                return RunAsync(command, Document);

            return Task.CompletedTask;
        }

        protected virtual void RefreshContext(Command command, TDocument document) { }
        protected virtual bool CanRun(Command command, TDocument document) => true;
        protected virtual void UpdateStatus(Command command, TDocument document) { }
        protected abstract Task RunAsync(Command command, TDocument document);
    }
}