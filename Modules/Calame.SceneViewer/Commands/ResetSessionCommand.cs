using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneViewer.Commands.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.SceneViewer.Commands
{
    [CommandDefinition]
    public class ResetSessionCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Reset Session";
        public override object IconKey => CalameIconKey.ResetSession;

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<ResetSessionCommand>
        {
            protected override Task RunAsync(Command command, SceneViewerViewModel document)
            {
                document.ResetSession();
                return Task.CompletedTask;
            }
        }
    }
}