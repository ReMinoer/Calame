using System.ComponentModel.Composition;
using Calame.Commands;
using Calame.Commands.Base;
using Calame.SceneGraph.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.SceneGraph.Commands
{
    [CommandHandler]
    public class SceneGraphCommand : OpenToolCommandBase<SceneGraphCommand.Definition, SceneGraphViewModel>
    {
        [CommandDefinition]
        public class Definition : CalameCommandDefinitionBase
        {
            public override string Name => "SceneGraph.Open";
            public override string Text => "Scene _Graph";
        }

        [ImportingConstructor]
        public SceneGraphCommand(IShell shell)
            : base(shell)
        {
        }
    }
}