using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneViewer.Commands.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands
{
    [CommandDefinition]
    public class FreeCameraCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Free Camera";
        public override object IconKey => CalameIconKey.FreeCamera;

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<FreeCameraCommand>
        {
            protected override void UpdateStatus(Command command, SceneViewerViewModel document)
            {
                base.UpdateStatus(command, document);
                command.Checked = document?.FreeCameraEnabled == true;
            }

            protected override void Run(SceneViewerViewModel document)
            {
                document.EnableFreeCamera();
            }
        }
    }
}