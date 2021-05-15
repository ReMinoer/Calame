using Calame.Commands.Base;
using Calame.Icons;
using Calame.Viewer.Commands.Base;
using Gemini.Framework.Commands;

namespace Calame.Viewer.Commands
{
    [CommandDefinition]
    public class ResetCameraCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Reset Camera";
        public override object IconKey => CalameIconKey.ResetCamera;

        [CommandHandler]
        public class CommandHandler : ViewerDocumentCommandHandlerBase<IViewerDocument, ResetCameraCommand>
        {
            protected override void Run(IViewerDocument document)
            {
                document.EnableFreeCamera();
                document.Viewer.EditorCamera.ShowTarget(document.Viewer.UserRoot);
            }
        }
    }
}