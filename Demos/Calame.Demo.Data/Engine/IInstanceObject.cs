using Glyph.Composition;
using Glyph.Core;

namespace Calame.Demo.Data.Engine
{
    public interface IInstanceObject : IGlyphComponent
    {
        SceneNode SceneNode { get; }
    }
}