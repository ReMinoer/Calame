using System.ComponentModel.Composition;
using Calame.Commands;
using Calame.PropertyGrid.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.PropertyGrid.Commands
{
    [CommandHandler]
    public class PropertyGridCommand : OpenToolCommandBase<PropertyGridCommand.Definition, PropertyGridViewModel>
    {
        [CommandDefinition]
        public class Definition : CalameCommandDefinitionBase
        {
            public override string Name => "PropertyGrid.Open";
            public override string Text => "_Property Grid";
        }

        [ImportingConstructor]
        public PropertyGridCommand(IShell shell)
            : base(shell)
        {
        }
    }
}