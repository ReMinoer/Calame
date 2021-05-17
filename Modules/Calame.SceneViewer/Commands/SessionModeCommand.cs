using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneViewer.ViewModels;
using Calame.Viewer.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands
{
    [CommandDefinition]
    public class SessionModeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Session Mode";
        public override object IconKey => CalameIconKey.SessionMode;

        [CommandHandler]
        public class CommandHandler : SwitchViewerModeCommandHandlerBase<SceneViewerViewModel.SessionModeModule, SessionModeCommand>
        {
        }
    }
}