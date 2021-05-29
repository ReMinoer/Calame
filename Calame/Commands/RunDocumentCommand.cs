using System;
using Calame.Commands.Base;
using Calame.Icons;
using Gemini.Framework.Commands;

namespace Calame.Commands
{
    [CommandDefinition]
    public class RunDocumentCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Run";
        public override object IconKey => CalameIconKey.Play;

        [CommandHandler]
        public class CommandHandler : ContextualDocumentCommandHandlerBase<IRunnableDocument, RunDocumentCommand>
        {
            protected override bool VisibleEvenIfNotUsed => true;
            protected override Type GetCommandDefinitionType(IRunnableDocument document) => document.RunCommandDefinitionType;
        }
    }
}