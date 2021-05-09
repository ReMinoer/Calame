using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneViewer.Commands.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands
{
    [CommandDefinition]
    public class NextFrameCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Next Frame";
        public override object IconKey => CalameIconKey.NextFrame;

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<NextFrameCommand>
        {
            protected override Task RunAsync(Command command, SceneViewerViewModel document)
            {
                document.Viewer.Runner.Engine.PauseOnNextFrame();
                return Task.CompletedTask;
            }
        }
    }
}