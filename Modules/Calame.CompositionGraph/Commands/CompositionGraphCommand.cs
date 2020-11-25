using Calame.Commands;
using Calame.Commands.Base;
using Calame.CompositionGraph.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.CompositionGraph.Commands
{
    [CommandDefinition]
    public class CompositionGraphCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Composition Graph";

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<CompositionGraphCommand, CompositionGraphViewModel>
        {
        }
    }
}