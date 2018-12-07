using Glyph.WpfInterop;

namespace Calame.Viewer
{
    public interface IViewerView
    {
        IWpfGlyphClient Client { get; }
    }
}