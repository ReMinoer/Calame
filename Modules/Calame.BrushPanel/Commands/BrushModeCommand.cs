using Calame.Commands.Base;
using Calame.Icons;
using Calame.Viewer.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.BrushPanel.Commands
{
    [CommandDefinition]
    public class BrushModeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Brush Mode";
        public override object IconKey => CalameIconKey.BrushMode;

        [CommandHandler]
        public class CommandHandler : SwitchViewerModeCommandHandlerBase<IBrushViewerModule, BrushModeCommand>
        {
        }
    }
}