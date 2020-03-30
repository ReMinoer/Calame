using Glyph.Composition;
using Glyph.Composition.Modelization;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data
{
    public interface IShapeData<out T> : IGlyphCreator<T>
        where T : IGlyphComponent
    {
        Vector2 Position { get; set; }
        Color Color { get; set; }
    }
}