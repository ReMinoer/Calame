using Calame.BrushPanel.ViewModels;
using Calame.Commands;
using Calame.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.BrushPanel.Commands
{
    [CommandDefinition]
    public class BrushPanelCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Brush Panel";

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<BrushPanelCommand, BrushPanelViewModel>
        {
        }
    }
}