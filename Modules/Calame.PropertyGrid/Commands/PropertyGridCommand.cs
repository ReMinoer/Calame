using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.PropertyGrid.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.PropertyGrid.Commands
{
    [CommandHandler]
    public class PropertyGridCommand : CommandHandlerBase<PropertyGridCommand.Definition>
    {
        [CommandDefinition]
        public class Definition : CommandDefinition
        {
            public const string CommandName = "PropertyGrid.Open";
            public override string Name => CommandName;
            public override string Text => "_Property Grid";
            public override string ToolTip => "_Property Grid";
        }

        private readonly IShell _shell;

        [ImportingConstructor]
        public PropertyGridCommand(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<PropertyGridViewModel>();
            return TaskUtility.Completed;
        }
    }
}