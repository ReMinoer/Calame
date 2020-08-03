using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;

namespace Calame.Demo.Data.Engine
{
    public class SpriteInstanceObject : GlyphObject, IInstanceObject
    {
        public SceneNode SceneNode { get; }
        public SpriteLoader SpriteLoader { get; }

        public SpriteInstanceObject(GlyphResolveContext context)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
            SpriteLoader = Add<SpriteLoader>();
            Add<SpriteRenderer>();
        }
    }
}