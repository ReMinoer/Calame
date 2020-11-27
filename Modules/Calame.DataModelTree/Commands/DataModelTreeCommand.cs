using Calame.Commands;
using Calame.Commands.Base;
using Calame.DataModelTree.ViewModels;
using Calame.Icons;
using Gemini.Framework.Commands;

namespace Calame.DataModelTree.Commands
{
    [CommandDefinition]
    public class DataModelTreeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Data Model Tree";
        public override object IconKey => CalameIconKey.DataModelTree;

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<DataModelTreeCommand, DataModelTreeViewModel>
        {
        }
    }
}