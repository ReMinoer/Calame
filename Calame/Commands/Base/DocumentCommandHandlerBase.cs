using System;
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

        protected override sealed bool CanRun()
        {
            return base.CanRun()
                && Document != null
                && CanRun(Document);
        }

        protected override sealed void UpdateStatus(Command command)
        {
            base.UpdateStatus(command);
            UpdateStatus(command, Document);
        }

        protected override sealed void Run()
        {
            if (Document != null)
                Run(Document);
        }

        protected virtual void RefreshContext(Command command, TDocument document) { }
        protected virtual bool CanRun(TDocument document) => true;
        protected virtual void UpdateStatus(Command command, TDocument document) { }
        protected abstract void Run(TDocument document);
    }
}