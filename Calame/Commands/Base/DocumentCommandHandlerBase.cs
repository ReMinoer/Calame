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

        protected DocumentCommandHandlerBase()
        {
            Shell = IoC.Get<IShell>();
        }

        protected override sealed bool CanRun(Command command)
        {
            return Shell.ActiveItem is TDocument document && CanRun(command, document);
        }

        protected override sealed Task RunAsync(Command command)
        {
            if (Shell.ActiveItem is TDocument activeDocument)
                return RunAsync(command, activeDocument);

            return Task.CompletedTask;
        }

        protected virtual bool CanRun(Command command, TDocument document) => true;
        protected abstract Task RunAsync(Command command, TDocument document);
    }
}