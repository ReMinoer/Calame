using Calame.DocumentContexts;
using Calame.Viewer.ViewModels;
using Gemini.Framework;
using Glyph.Composition;
using Glyph.Engine;

namespace Calame.Viewer
{
    public interface IViewerDocument : IDocument, IViewerViewModelOwner,
        IDocumentContext<ViewerViewModel>,
        IDocumentContext<GlyphEngine>,
        IDocumentContext<IRootsContext>,
        IDocumentContext<IRootComponentsContext>,
        IDocumentContext<IRootScenesContext>,
        IDocumentContext<IRootInteractivesContext>,
        IDocumentContext<ISelectionCommandContext>,
        IDocumentContext<ISelectionCommandContext<IGlyphComponent>>,
        ISelectionCommandContext<IGlyphComponent>
    {
        ViewerViewModel Viewer { get; }
        bool DebugMode { get; set; }
        void EnableFreeCamera();
    }
}