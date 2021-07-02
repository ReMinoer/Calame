using Calame.Viewer;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.DataModelViewer
{
    public interface IEditorContext : IDefaultCameraTarget
    {
        GlyphEngine Engine { get; }
        GlyphObject EditorRoot { get; }
        new IBoxedComponent DefaultCameraTarget { get; set; }
    }
}