using Calame.BrushPanel.ViewModels;
using Calame.Commands;
using Calame.Commands.Base;
using Calame.Icons;
using Gemini.Framework.Commands;

namespace Calame.BrushPanel.Commands
{
    [CommandDefinition]
    public class BrushPanelCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Brush Panel";
        public override object IconKey => CalameIconKey.BrushPanel;

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<BrushPanelCommand, BrushPanelViewModel>
        {
        }
    }
}