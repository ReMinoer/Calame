using Diese.Collections;
using Glyph;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Injection.Binding;

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
            PropertyBindings.AddProperty(x => x.Position, x => x.Components.FirstOfType<SceneNode>().Position);
            PropertyBindings.AddProperty(x => x.Color, x => x.Components.FirstOfType<SpriteTransformer>().Color);
            PropertyBindings.AddProperty(
                x => x.Width,
                x => x.Components.FirstOfType<SpriteTransformer>().Scale,
                (width, m, v) => v.Components.FirstOfType<SpriteTransformer>().Scale.SetX(width / v.Components.FirstOfType<FilledRectangleSprite>().Width));
            PropertyBindings.AddProperty(
                x => x.Height,
                x => x.Components.FirstOfType<SpriteTransformer>().Scale,
                (height, m, v) => v.Components.FirstOfType<SpriteTransformer>().Scale.SetY(height / v.Components.FirstOfType<FilledRectangleSprite>().Height));
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