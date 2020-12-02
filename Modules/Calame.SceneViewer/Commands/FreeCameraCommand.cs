using System.Threading.Tasks;
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
            public override bool ShowOnlyIfEnabled => true;

            protected override void UpdateStatus(Command command)
            {
                command.Checked = (Shell.ActiveItem as SceneViewerViewModel)?.FreeCameraEnabled == true;
            }

            protected override Task RunAsync(Command command, SceneViewerViewModel document)
            {
                document.EnableFreeCamera();
                return Task.CompletedTask;
            }
        }
    }
}