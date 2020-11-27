using Calame.Commands;
using Calame.Commands.Base;
using Calame.CompositionGraph.ViewModels;
using Calame.Icons;
using Gemini.Framework.Commands;

namespace Calame.CompositionGraph.Commands
{
    [CommandDefinition]
    public class CompositionGraphCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Composition Graph";
        public override object IconKey => CalameIconKey.CompositionGraph;

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<CompositionGraphCommand, CompositionGraphViewModel>
        {
        }
    }
}