using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.InteractionTree.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.InteractionTree.Commands
{
    [CommandHandler]
    public class InteractionTreeCommand : CommandHandlerBase<InteractionTreeCommand.Definition>
    {
        [CommandDefinition]
        public class Definition : CommandDefinition
        {
            public const string CommandName = "InteractionTree.Open";
            public override string Name => CommandName;
            public override string Text => "_Interaction Tree";
            public override string ToolTip => "_Interaction Tree";
        }

        private readonly IShell _shell;

        [ImportingConstructor]
        public InteractionTreeCommand(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<InteractionTreeViewModel>();
            return TaskUtility.Completed;
        }
    }
}