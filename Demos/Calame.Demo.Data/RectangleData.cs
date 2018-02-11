using Diese.Collections;
using Glyph;
using Glyph.Binding;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data
{
    public class RectangleData : BindedData<RectangleData, GlyphObject>, IShapeData
    {
        public Vector2 Position { get; set; }
        public float Width { get; set; } = 100;
        public float Height { get; set; } = 100;
        public Color Color { get; set; } = Color.White;

        static RectangleData()
        {
            Bindings.Add(x => x.Position, x => x.Components.First<SceneNode>().Position);
            Bindings.Add(x => x.Color, x => x.Components.First<SpriteTransformer>().Color);
            Bindings.Add(
                x => x.Width,
                x => x.Components.First<SpriteTransformer>().Scale,
                (width, m, v) => v.Components.First<SpriteTransformer>().Scale.SetX(width / v.Components.First<FilledRectangleSprite>().Width));
            Bindings.Add(
                x => x.Height,
                x => x.Components.First<SpriteTransformer>().Scale,
                (height, m, v) => v.Components.First<SpriteTransformer>().Scale.SetY(height / v.Components.First<FilledRectangleSprite>().Height));
        }

        protected override GlyphObject New()
        {
            GlyphObject glyphObject = base.New();
            
            glyphObject.Add<SceneNode>();
            glyphObject.Add<FilledRectangleSprite>().Size = new Point(100, 100);
            glyphObject.Add<SpriteTransformer>();
            glyphObject.Add<SpriteRenderer>();
            
            return glyphObject;
        }
    }
}