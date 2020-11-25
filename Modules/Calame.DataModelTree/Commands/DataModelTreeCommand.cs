using System.ComponentModel.Composition;
using Calame.Commands;
using Calame.Commands.Base;
using Calame.DataModelTree.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.DataModelTree.Commands
{
    [CommandHandler]
    public class DataModelTreeCommand : OpenToolCommandBase<DataModelTreeCommand.Definition, DataModelTreeViewModel>
    {
        [CommandDefinition]
        public class Definition : CalameCommandDefinitionBase
        {
            public override string Text => "_Data Model Tree";
        }

        [ImportingConstructor]
        public DataModelTreeCommand(IShell shell)
            : base(shell)
        {
        }
    }
}