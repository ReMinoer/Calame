using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.Viewer.Commands.Base;
using Gemini.Framework.Commands;
using Glyph.Core;

namespace Calame.Viewer.Commands
{
    [CommandDefinition]
    public class FocusCameraCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Focus Camera";
        public override object IconKey => CalameIconKey.FocusCamera;

        [CommandHandler]
        public class CommandHandler : ViewerDocumentCommandHandlerBase<IViewerDocument, FocusCameraCommand>
        {
            protected override bool CanRun(Command command, IViewerDocument document)
            {
                return base.CanRun(command, document)
                    && document.Viewer.LastSelection?.Item is IBoxedComponent;
            }

            protected override Task RunAsync(Command command, IViewerDocument document)
            {
                document.EnableFreeCamera();
                document.Viewer.EditorCamera.ShowTarget((IBoxedComponent)document.Viewer.LastSelection.Item);
                return Task.CompletedTask;
            }
        }
    }
}