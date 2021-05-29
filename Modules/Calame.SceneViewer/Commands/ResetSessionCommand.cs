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
        public override object IconKey => CalameIconKey.Reset;

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<ResetSessionCommand>
        {
            protected override void Run(SceneViewerViewModel document)
            {
                document.ResetSessionAsync().Wait();
            }
        }
    }
}