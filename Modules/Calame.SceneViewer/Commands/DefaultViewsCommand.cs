using System.Threading.Tasks;
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
            public override bool ShowOnlyIfEnabled => true;

            protected override void UpdateStatus(Command command)
            {
                command.Checked = (Shell.ActiveItem as SceneViewerViewModel)?.FreeCameraEnabled == false;
            }

            protected override Task RunAsync(Command command, SceneViewerViewModel document)
            {
                document.EnableDefaultCamera();
                return Task.CompletedTask;
            }
        }
    }
}