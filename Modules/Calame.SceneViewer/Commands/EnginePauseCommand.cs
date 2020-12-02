using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneViewer.Commands.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;
using Glyph.Engine;

namespace Calame.SceneViewer.Commands
{
    [CommandDefinition]
    public class EnginePauseCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Pause";
        public override object IconKey => CalameIconKey.Pause;

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<EnginePauseCommand>
        {
            public override bool ShowOnlyIfEnabled => true;

            protected override void UpdateStatus(Command command)
            {
                bool isPaused = (Shell.ActiveItem as SceneViewerViewModel)?.Viewer?.Runner?.Engine?.IsPaused == true;
                if ((bool?)command.Tag == isPaused)
                    return;

                command.Tag = isPaused;
                command.Checked = isPaused;
            }

            protected override Task RunAsync(Command command, SceneViewerViewModel document)
            {
                GlyphEngine engine = document.Viewer.Runner.Engine;

                if (!engine.IsPaused)
                    engine.Pause();
                else
                    engine.Start();

                return Task.CompletedTask;
            }
        }
    }
}