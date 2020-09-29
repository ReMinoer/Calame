using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Tools.Transforming;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data.Data
{
    public interface IInstanceData<out T> : IGlyphCreator<T>, IPositionController, IRotationController, IScaleController
        where T : IGlyphComponent
    {
        Vector2 LocalPosition { get; set; }
        float LocalRotation { get; set; }
        float LocalScale { get; set; }
    }
}