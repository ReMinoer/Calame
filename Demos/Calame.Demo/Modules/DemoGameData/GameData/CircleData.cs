using Diese.Collections;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Modules.DemoGameData.GameData
{
    public class CircleData : GlyphCreatorBase<GlyphObject>
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; } = 50;
        public Color Color { get; set; } = Color.White;

        public override GlyphObject Create()
        {
            var glyphObject = Injector.Resolve<GlyphObject>();
            glyphObject.Add<SceneNode>();
            glyphObject.Add<FilledCircleSprite>().Radius = 50;
            glyphObject.Add<SpriteTransformer>();
            glyphObject.Add<SpriteRenderer>();

            Configure(glyphObject);
            return glyphObject;
        }

        public void Configure(GlyphObject obj)
        {
            obj.Components.First<SceneNode>().Position = Position;
            
            var spriteTransformer = obj.Components.First<SpriteTransformer>();
            spriteTransformer.Scale = Radius / obj.Components.First<FilledCircleSprite>().Radius * Vector2.One;
            spriteTransformer.Color = Color;
        }
    }
}