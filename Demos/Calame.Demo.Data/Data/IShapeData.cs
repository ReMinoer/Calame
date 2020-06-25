using Glyph.Composition;
using Glyph.Composition.Modelization;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data.Data
{
    public interface IShapeData<out T> : IGlyphCreator<T>
        where T : IGlyphComponent
    {
        Color Color { get; set; }
    }
}