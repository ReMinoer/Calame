using System;
using Calame.Commands.Base;
using Calame.Icons;
using Gemini.Framework.Commands;

namespace Calame.Commands
{
    [CommandDefinition]
    public class RunDocumentAlternativeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Alternative Run";
        public override object IconKey => null;

        [CommandHandler]
        public class CommandHandler : ContextualDocumentCommandHandlerBase<IRunnableDocument, RunDocumentAlternativeCommand>
        {
            protected override bool VisibleEvenIfNotUsed => false;
            protected override Type GetCommandDefinitionType(IRunnableDocument document) => document.RunAlternativeCommandDefinitionType;
        }
    }
}