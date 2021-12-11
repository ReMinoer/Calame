using Calame.Commands.Base;
using Calame.Icons;
using Calame.ViewGraph.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.ViewGraph.Commands
{
    [CommandDefinition]
    public class ViewGraphCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_View Graph";
        public override object IconKey => CalameIconKey.ViewGraph;

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<ViewGraphCommand, ViewGraphViewModel>
        {
        }
    }
}