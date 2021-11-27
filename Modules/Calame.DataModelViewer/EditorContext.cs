using Glyph.Core;
using Glyph.Engine;

namespace Calame.DataModelViewer
{
    public class EditorContext : IEditorContext
    {
        public GlyphEngine Engine { get; }
        public GlyphObject EditorRoot { get; }
        public IBoxedComponent DefaultCameraTarget { get; set; }

        public EditorContext(GlyphEngine engine, GlyphObject editorRoot, IBoxedComponent defaultCameraTarget)
        {
            Engine = engine;
            EditorRoot = editorRoot;
            DefaultCameraTarget = defaultCameraTarget;
        }
    }
}