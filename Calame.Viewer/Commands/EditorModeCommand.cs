using Calame.Commands.Base;
using Calame.Icons;
using Calame.Viewer.Commands.Base;
using Calame.Viewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.Viewer.Commands
{
    [CommandDefinition]
    public class EditorModeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Editor Mode";
        public override object IconKey => CalameIconKey.EditorMode;

        [CommandHandler]
        public class CommandHandler : SwitchViewerModeCommandHandlerBase<ViewerViewModel.EditorModeModule, EditorModeCommand>
        {
        }
    }
}