using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.BrushPanel.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.BrushPanel.Commands
{
    [CommandHandler]
    public class BrushPanelCommand : CommandHandlerBase<BrushPanelCommand.Definition>
    {
        [CommandDefinition]
        public class Definition : CommandDefinition
        {
            public const string CommandName = "BrushPanel.Open";
            public override string Name => CommandName;
            public override string Text => "_Brush Panel";
            public override string ToolTip => "_Brush Panel";
        }

        private readonly IShell _shell;

        [ImportingConstructor]
        public BrushPanelCommand(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<BrushPanelViewModel>();
            return TaskUtility.Completed;
        }
    }
}