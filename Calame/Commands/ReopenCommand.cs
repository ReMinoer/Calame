using System.Linq;
using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.Commands
{
    [CommandDefinition]
    public class ReopenCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Re-Open";
        public override object IconKey => CalameIconKey.Reopen;

        [CommandHandler]
        public class CommandHandler : DocumentCommandHandlerBase<IPersistedDocument, ReopenCommand>
        {
            private readonly IEditorProvider[] _editorProviders;

            protected CommandHandler()
            {
                _editorProviders = IoC.GetAllInstances(typeof(IEditorProvider)).Cast<IEditorProvider>().ToArray();
            }

            protected override void Run(IPersistedDocument document)
            {
                TaskHandler.HandleOnUIThread(async () =>
                {
                    if (document is CalamePersistedDocumentBase persistedDocument
                        && await persistedDocument.TrySaveBeforeCloseAsync() is null)
                        return;

                    await Shell.CloseDocumentAsync(document);
                    await Shell.OpenFileAsync(document.FilePath, _editorProviders);
                });
            }
        }
    }
}