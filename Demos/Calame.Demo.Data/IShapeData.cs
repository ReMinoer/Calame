using Glyph.Composition.Modelization;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data
{
    public interface IShapeData : IGlyphCreator<GlyphObject>
    {
        Vector2 Position { get; set; }
        Color Color { get; set; }
    }
}