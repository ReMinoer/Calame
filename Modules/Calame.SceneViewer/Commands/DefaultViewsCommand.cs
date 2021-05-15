using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneViewer.Commands.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands
{
    [CommandDefinition]
    public class DefaultCameraCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Default Camera";
        public override object IconKey => CalameIconKey.DefaultCamera;

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<DefaultCameraCommand>
        {
            protected override void UpdateStatus(Command command, SceneViewerViewModel document)
            {
                base.UpdateStatus(command, document);
                command.Checked = document?.FreeCameraEnabled == false;
            }

            protected override void Run(SceneViewerViewModel document)
            {
                document.EnableDefaultCamera();
            }
        }
    }
}