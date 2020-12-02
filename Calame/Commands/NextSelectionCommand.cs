using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Gemini.Framework;
using Gemini.Framework.Commands;

namespace Calame.Commands
{
    [CommandDefinition]
    public class NextSelectionCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Next Selection";
        public override object IconKey => CalameIconKey.Next;

        [CommandHandler]
        public class CommandHandler : DocumentCommandHandlerBase<IDocument, NextSelectionCommand>
        {
            private readonly SelectionHistoryManager _selectionHistoryManager;

            [ImportingConstructor]
            public CommandHandler(SelectionHistoryManager selectionHistoryManager)
            {
                _selectionHistoryManager = selectionHistoryManager;
            }

            protected override bool CanRun(Command command, IDocument document)
            {
                return _selectionHistoryManager.CurrentDocumentHistory.HasNext;
            }

            protected override async Task RunAsync(Command command, IDocument document)
            {
                await _selectionHistoryManager.CurrentDocumentHistory.SelectNextAsync();
            }
        }
    }
}