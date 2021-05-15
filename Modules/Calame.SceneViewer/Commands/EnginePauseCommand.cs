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
        public override string Text => "_Pause";
        public override object IconKey => CalameIconKey.Pause;

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<EnginePauseCommand>
        {
            protected override void UpdateStatus(Command command, SceneViewerViewModel document)
            {
                base.UpdateStatus(command, document);
                command.Checked = document?.Viewer?.Runner?.Engine?.IsPaused == true;
            }

            protected override void Run(SceneViewerViewModel document)
            {
                GlyphEngine engine = document.Viewer.Runner.Engine;

                if (!engine.IsPaused)
                    engine.Pause();
                else
                    engine.Start();
            }
        }
    }
}