using System.ComponentModel.Composition;
using Calame.Commands;
using Calame.CompositionGraph.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.CompositionGraph.Commands
{
    [CommandHandler]
    public class CompositionGraphCommand : OpenToolCommandBase<CompositionGraphCommand.Definition, CompositionGraphViewModel>
    {
        [CommandDefinition]
        public class Definition : CalameCommandDefinitionBase
        {
            public override string Name => "CompositionGraph.Open";
            public override string Text => "_Composition Graph";
        }

        [ImportingConstructor]
        public CompositionGraphCommand(IShell shell)
            : base(shell)
        {
        }
    }
}