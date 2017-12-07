using Diese.Collections;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Modules.DemoGameData.GameData
{
    public class RectangleData : GlyphCreatorBase<GlyphObject>
    {
        public Vector2 Position { get; set; }
        public float Width { get; set; } = 100;
        public float Height { get; set; } = 100;
        public Color Color { get; set; } = Color.White;

        public override GlyphObject Create()
        {
            var glyphObject = Injector.Resolve<GlyphObject>();
            glyphObject.Add<SceneNode>();
            glyphObject.Add<FilledRectangleSprite>().Size = new Point(100, 100);
            glyphObject.Add<SpriteTransformer>();
            glyphObject.Add<SpriteRenderer>();

            Configure(glyphObject);
            return glyphObject;
        }

        public void Configure(GlyphObject obj)
        {
            obj.Components.First<SceneNode>().Position = Position;

            var spriteTransformer = obj.Components.First<SpriteTransformer>();
            spriteTransformer.Scale = new Vector2(Width, Height) / obj.Components.First<FilledRectangleSprite>().Size.ToVector2();
            spriteTransformer.Color = Color;
        }
    }
}