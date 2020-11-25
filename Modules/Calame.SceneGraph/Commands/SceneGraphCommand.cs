using Calame.Commands;
using Calame.Commands.Base;
using Calame.SceneGraph.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneGraph.Commands
{
    [CommandDefinition]
    public class SceneGraphCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Scene _Graph";

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<SceneGraphCommand, SceneGraphViewModel>
        {
        }
    }
}