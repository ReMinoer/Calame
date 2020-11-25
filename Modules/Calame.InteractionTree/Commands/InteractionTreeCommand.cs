using System.ComponentModel.Composition;
using Calame.Commands;
using Calame.Commands.Base;
using Calame.InteractionTree.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.InteractionTree.Commands
{
    [CommandHandler]
    public class InteractionTreeCommand : OpenToolCommandBase<InteractionTreeCommand.Definition, InteractionTreeViewModel>
    {
        [CommandDefinition]
        public class Definition : CalameCommandDefinitionBase
        {
            public override string Name => "InteractionTree.Open";
            public override string Text => "_Interaction Tree";
        }

        [ImportingConstructor]
        public InteractionTreeCommand(IShell shell)
            : base(shell)
        {
        }
    }
}