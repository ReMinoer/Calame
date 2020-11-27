using Calame.Commands;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.PropertyGrid.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.PropertyGrid.Commands
{
    [CommandDefinition]
    public class PropertyGridCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Property Grid";
        public override object IconKey => CalameIconKey.PropertyGrid;

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<PropertyGridCommand, PropertyGridViewModel>
        {
        }
    }
}