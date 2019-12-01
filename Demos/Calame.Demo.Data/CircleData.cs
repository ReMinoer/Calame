using Diese.Collections;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Injection.Binding;

namespace Calame.Demo.Data
{
    public class CircleData : BindedData<CircleData, GlyphObject>, IShapeData
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; } = 50;
        public Color Color { get; set; } = Color.White;

        static CircleData()
        {
            PropertyBindings.AddProperty(x => x.Position, x => x.Components.FirstOfType<SceneNode>().Position);
            PropertyBindings.AddProperty(x => x.Color, x => x.Components.FirstOfType<SpriteTransformer>().Color);
            PropertyBindings.AddProperty(
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