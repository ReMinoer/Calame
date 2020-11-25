using System.ComponentModel.Composition;
using Calame.BrushPanel.ViewModels;
using Calame.Commands;
using Calame.Commands.Base;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.BrushPanel.Commands
{
    [CommandHandler]
    public class BrushPanelCommand : OpenToolCommandBase<BrushPanelCommand.Definition, BrushPanelViewModel>
    {
        [CommandDefinition]
        public class Definition : CalameCommandDefinitionBase
        {
            public override string Name => "BrushPanel.Open";
            public override string Text => "_Brush Panel";
        }

        [ImportingConstructor]
        public BrushPanelCommand(IShell shell)
            : base(shell)
        {
        }
    }
}