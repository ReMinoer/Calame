using Calame.DocumentContexts;
using Calame.Viewer.ViewModels;
using Gemini.Framework;
using Glyph.Composition;

namespace Calame.Viewer
{
    public interface IViewerDocument : IDocument, IViewerViewModelOwner,
        IDocumentContext<ViewerViewModel>,
        IDocumentContext<IContentLibraryContext>,
        IDocumentContext<IRawContentLibraryContext>,
        IDocumentContext<IRootsContext>,
        IDocumentContext<IRootComponentsContext>,
        IDocumentContext<IRootScenesContext>,
        IDocumentContext<IViewsContext>,
        IDocumentContext<IRootInteractivesContext>,
        IDocumentContext<ISelectionContext>,
        IDocumentContext<ISelectionContext<IGlyphComponent>>,
        ISelectionContext<IGlyphComponent>,
        IDefaultCameraTarget
    {
        ViewerViewModel Viewer { get; }
        bool DebugMode { get; set; }
        void EnableFreeCamera();
    }
}