using Glyph.Engine;

namespace Calame.Viewer
{
    public interface IViewerDocument : IViewerViewModelOwner, IDocumentContext<GlyphEngine>, IDocumentContext<ViewerViewModel>, IDocumentContext<IComponentFilter>
    {
        
    }
}