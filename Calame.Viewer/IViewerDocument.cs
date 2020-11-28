using Calame.Viewer.ViewModels;
using Gemini.Framework;
using Glyph.Engine;

namespace Calame.Viewer
{
    public interface IViewerDocument : IDocument, IViewerViewModelOwner, IDocumentContext<GlyphEngine>, IDocumentContext<ViewerViewModel>, IDocumentContext<IComponentFilter>
    {
        ViewerViewModel Viewer { get; }
    }
}