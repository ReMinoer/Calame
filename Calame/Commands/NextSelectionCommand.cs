using System.ComponentModel.Composition;
using Calame.Commands.Base;
using Calame.Icons;
using Gemini.Framework;
using Gemini.Framework.Commands;

namespace Calame.Commands
{
    [CommandDefinition]
    public class NextSelectionCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Next Selection";
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

            protected override bool CanRun(IDocument document)
            {
                return _selectionHistoryManager.CurrentDocumentHistory?.HasNext ?? false;
            }

            protected override void Run(IDocument document)
            {
                _selectionHistoryManager.CurrentDocumentHistory.SelectNextAsync().Wait();
            }
        }
    }
}