using Fingear;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.SceneViewer
{
    public interface ISessionContext
    {
        GlyphEngine Engine { get; }
        IView RootView { get; }
        GlyphObject EditorRoot { get; }
        IInteractiveContainer SessionInteractive { get; } 
    }
}