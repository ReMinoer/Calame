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
        private TDocument _document;

        protected DocumentCommandHandlerBase()
        {
            Shell = IoC.Get<IShell>();
            Shell.ActiveDocumentChanged += OnActiveDocumentChanged;
        }

        private void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            _document = Shell.ActiveItem as TDocument;
        }

        protected override void UpdateStatus(Command command)
        {
            base.UpdateStatus(command);
            UpdateStatus(command, _document);
        }

        protected override sealed bool CanRun(Command command)
        {
            return base.CanRun(command)
                && _document != null
                && CanRun(command, _document);
        }

        protected override sealed Task RunAsync(Command command)
        {
            if (_document != null)
                return RunAsync(command, _document);

            return Task.CompletedTask;
        }

        protected virtual void UpdateStatus(Command command, TDocument document) { }
        protected virtual bool CanRun(Command command, TDocument document) => true;
        protected abstract Task RunAsync(Command command, TDocument document);
    }
}