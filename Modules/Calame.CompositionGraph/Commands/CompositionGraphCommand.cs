using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.CompositionGraph.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.CompositionGraph.Commands
{
    [CommandHandler]
    public class CompositionGraphCommand : CommandHandlerBase<CompositionGraphCommand.Definition>
    {
        [CommandDefinition]
        public class Definition : CommandDefinition
        {
            public const string CommandName = "Demos.CompositionGraph";
            public override string Name => CommandName;
            public override string Text => "_Composition Graph";
            public override string ToolTip => "_Composition Graph";
        }

        private readonly IShell _shell;

        [ImportingConstructor]
        public CompositionGraphCommand(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<CompositionGraphViewModel>();
            return TaskUtility.Completed;
        }
    }
}