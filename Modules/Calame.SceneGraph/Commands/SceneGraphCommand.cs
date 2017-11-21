using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.SceneGraph.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.SceneGraph.Commands
{
    [CommandHandler]
    public class SceneGraphCommand : CommandHandlerBase<SceneGraphCommand.Definition>
    {
        [CommandDefinition]
        public class Definition : CommandDefinition
        {
            public const string CommandName = "Demos.SceneGraph";
            public override string Name => CommandName;
            public override string Text => "Scene _Graph";
            public override string ToolTip => "Scene _Graph";
        }

        private readonly IShell _shell;

        [ImportingConstructor]
        public SceneGraphCommand(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<SceneGraphViewModel>();
            return TaskUtility.Completed;
        }
    }
}