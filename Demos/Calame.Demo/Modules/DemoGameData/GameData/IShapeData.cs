using Glyph.Core;
using Glyph.Modelization;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Modules.DemoGameData.GameData
{
    public interface IShapeData : IBindedGlyphCreator<GlyphObject>
    {
        Vector2 Position { get; set; }
        Color Color { get; set; }
    }
}