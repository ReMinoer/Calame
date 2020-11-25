using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.SceneViewer.Commands.Base;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework.Commands;
using Glyph.Engine;

namespace Calame.SceneViewer.Commands
{
    [CommandDefinition]
    public class EnginePauseResumeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Pause/Resume";

        [Export]
        static public CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<EnginePauseResumeCommand>(new KeyGesture(Key.Space));

        [CommandHandler]
        public class CommandHandler : SceneViewerCommandHandlerBase<EnginePauseResumeCommand>
        {
            private readonly IIconProvider _iconProvider;
            private readonly IIconDescriptor<Command> _iconDescriptor;

            [ImportingConstructor]
            public CommandHandler(IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            {
                _iconProvider = iconProvider;
                _iconDescriptor = iconDescriptorManager.GetDescriptor<Command>();
            }

            protected override void UpdateStatus(Command command)
            {
                bool isPaused = (Shell.ActiveItem as SceneViewerViewModel)?.Viewer?.Runner?.Engine?.IsPaused == true;
                if ((bool?)command.Tag == isPaused)
                    return;

                command.Tag = isPaused;
                command.Text = isPaused ? "Resume" : "Pause";
                command.IconSource = _iconProvider.GetUri(_iconDescriptor.GetIcon(command), 16);
            }

            protected override bool CanRun(Command command, SceneViewerViewModel document)
            {
                GlyphEngine engine = document.Viewer?.Runner?.Engine;
                return engine != null && engine.IsStarted;
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