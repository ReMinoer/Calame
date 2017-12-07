using Glyph.Engine;

namespace Calame.SceneViewer
{
    public interface ISession
    {
        string DisplayName { get; }
        string ContentPath { get; }
        void PrepareSession(GlyphEngine engine);
    }
}