using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.DataModelTree.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.DataModelTree.Commands
{
    [CommandHandler]
    public class DataModelTreeCommand : CommandHandlerBase<DataModelTreeCommand.Definition>
    {
        [CommandDefinition]
        public class Definition : CommandDefinition
        {
            public const string CommandName = "DataModelTree.Open";
            public override string Name => CommandName;
            public override string Text => "_Data Model Tree";
            public override string ToolTip => "_Data Model Tree";
        }

        private readonly IShell _shell;

        [ImportingConstructor]
        public DataModelTreeCommand(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<DataModelTreeViewModel>();
            return TaskUtility.Completed;
        }
    }
}