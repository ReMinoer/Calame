using Diese.Collections;
using Glyph.Binding;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data
{
    public class CircleData : BindedData<CircleData, GlyphObject>, IShapeData
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; } = 50;
        public Color Color { get; set; } = Color.White;

        static CircleData()
        {
            Bindings.Add(x => x.Position, x => x.Components.FirstOfType<SceneNode>().Position);
            Bindings.Add(x => x.Color, x => x.Components.FirstOfType<SpriteTransformer>().Color);
            Bindings.Add(
                x => x.Radius,
                x => x.Components.FirstOfType<SpriteTransformer>().Scale,
                (radius, m, v) => radius / v.Components.FirstOfType<FilledCircleSprite>().Radius * Vector2.One);
        }

        protected override GlyphObject New()
        {
            GlyphObject glyphObject = base.New();
            
            glyphObject.Add<SceneNode>();
            glyphObject.Add<FilledCircleSprite>().Radius = 50;
            glyphObject.Add<SpriteTransformer>();
            glyphObject.Add<SpriteRenderer>();
            
            return glyphObject;
        }
    }
}