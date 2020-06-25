using Glyph.Core;
using Glyph.Graphics.Renderer;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data.Engine.Base
{
    public abstract class PrimitiveObjectBase : GlyphObject
    {
        private readonly SceneNode _sceneNode;
        protected readonly PrimitiveRenderer PrimitiveRenderer;

        public abstract Color Color { get; set; }

        public Vector2 Position
        {
            get => _sceneNode.Position;
            set => _sceneNode.Position = value;
        }

        public PrimitiveObjectBase(GlyphResolveContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();
            PrimitiveRenderer = Add<PrimitiveRenderer>();
        }
    }
}