using Calame.Commands;
using Calame.Commands.Base;
using Calame.InteractionTree.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.InteractionTree.Commands
{
    [CommandDefinition]
    public class InteractionTreeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Interaction Tree";

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<InteractionTreeCommand, InteractionTreeViewModel>
        {
        }
    }
}