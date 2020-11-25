using Calame.Commands;
using Calame.Commands.Base;
using Calame.DataModelTree.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.DataModelTree.Commands
{
    [CommandDefinition]
    public class DataModelTreeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Data Model Tree";

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<DataModelTreeCommand, DataModelTreeViewModel>
        {
        }
    }
}