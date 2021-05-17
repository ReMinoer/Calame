using System.Linq;
using System.Text;
using System.Windows;
using Calame.Commands.Base;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.ViewModels;
using Gemini.Modules.Shell.Views;

namespace Calame.Commands
{
    [CommandDefinition]
    public class ReloadLayoutCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Reload Layout";
        public override object IconKey => null;

        [CommandHandler]
        public class CommandHandler : LayoutCommandHandlerBase<ReloadLayoutCommand>
        {
            protected override void Run(ILayoutItemStatePersister layoutItemStatePersister, ShellViewModel shellViewModel)
            {
                foreach (IDocument document in shellViewModel.Documents.Where(x => x != shellViewModel.ActiveItem).ToArray())
                    shellViewModel.CloseDocumentAsync(document).Wait();

                if (shellViewModel.ActiveItem != null)
                    shellViewModel.CloseDocumentAsync(shellViewModel.ActiveItem).Wait();

                if (shellViewModel.Documents.Count > 0)
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine("Failed to restore default layout because following documents are not closed:");
                    foreach (IDocument document in shellViewModel.Documents)
                    {
                        messageBuilder.AppendLine();
                        messageBuilder.Append($"- {document.DisplayName}");
                    }

                    MessageBox.Show(messageBuilder.ToString(), "Reload Layout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (ITool tool in shellViewModel.Tools)
                    tool.IsVisible = false;

                layoutItemStatePersister.LoadState(shellViewModel, (IShellView)shellViewModel.GetView(), shellViewModel.StateFile);
            }
        }
    }
}