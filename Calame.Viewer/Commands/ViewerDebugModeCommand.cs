using Calame.Commands.Base;
using Calame.Icons;
using Calame.Viewer.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.Viewer.Commands
{
    [CommandDefinition]
    public class ViewerDebugModeCommand : CalameCommandDefinitionBase
    {
        public override string Text => "Viewer _Debug Mode";
        public override object IconKey => CalameIconKey.ViewerDebugMode;

        [CommandHandler]
        public class CommandHandler : ViewerDocumentCommandHandlerBase<IViewerDocument, ViewerDebugModeCommand>
        {
            protected override void UpdateStatus(Command command, IViewerDocument document)
            {
                base.UpdateStatus(command, document);
                command.Checked = document?.DebugMode ?? false;
            }

            protected override void Run(IViewerDocument document)
            {
                document.DebugMode = !document.DebugMode;
            }
        }
    }
}