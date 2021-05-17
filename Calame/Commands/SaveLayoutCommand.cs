using Calame.Commands.Base;
using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.ViewModels;
using Gemini.Modules.Shell.Views;

namespace Calame.Commands
{
    [CommandDefinition]
    public class SaveLayoutCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Save Layout";
        public override object IconKey => null;

        [CommandHandler]
        public class CommandHandler : LayoutCommandHandlerBase<SaveLayoutCommand>
        {
            protected override void Run(ILayoutItemStatePersister layoutItemStatePersister, ShellViewModel shellViewModel)
            {
                layoutItemStatePersister.SaveState(shellViewModel, (IShellView)shellViewModel.GetView(), shellViewModel.StateFile);
            }
        }
    }
}