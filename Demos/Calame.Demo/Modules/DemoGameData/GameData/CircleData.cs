using Diese.Collections;
using Glyph.Binding;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Modelization;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Modules.DemoGameData.GameData
{
    public class CircleData : BindedData<CircleData, GlyphObject>, IShapeData
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; } = 50;
        public Color Color { get; set; } = Color.White;

        static CircleData()
        {
            Bindings.Add(x => x.Position, x => x.Components.First<SceneNode>().Position);
            Bindings.Add(x => x.Color, x => x.Components.First<SpriteTransformer>().Color);
            Bindings.Add(
                x => x.Radius,
                x => x.Components.First<SpriteTransformer>().Scale,
                (radius, m, v) => radius / v.Components.First<FilledCircleSprite>().Radius * Vector2.One,
                (scale, m, v) => scale.X * v.Components.First<FilledCircleSprite>().Radius);
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