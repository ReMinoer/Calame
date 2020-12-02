using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneGraph.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneGraph.Commands
{
    [CommandDefinition]
    public class SceneGraphCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Scene _Graph";
        public override object IconKey => CalameIconKey.SceneGraph;

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<SceneGraphCommand, SceneGraphViewModel>
        {
        }
    }
}