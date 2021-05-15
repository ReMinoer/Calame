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
            protected override bool CanRun(IViewerDocument document)
            {
                return base.CanRun(document)
                    && document.Viewer.LastSelection?.Item is IBoxedComponent;
            }

            protected override void Run(IViewerDocument document)
            {
                document.EnableFreeCamera();
                document.Viewer.EditorCamera.ShowTarget((IBoxedComponent)document.Viewer.LastSelection.Item);
            }
        }
    }
}