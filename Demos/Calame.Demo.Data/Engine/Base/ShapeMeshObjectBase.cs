using Glyph.Core;
using Glyph.Graphics.Renderer;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data.Engine.Base
{
    public abstract class ShapeMeshObjectBase : GlyphObject
    {
        private readonly SceneNode _sceneNode;
        protected readonly MeshRenderer MeshRenderer;

        public abstract Color Color { get; set; }

        public Vector2 Position
        {
            get => _sceneNode.Position;
            set => _sceneNode.Position = value;
        }

        public ShapeMeshObjectBase(GlyphResolveContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();
            MeshRenderer = Add<MeshRenderer>();
        }
    }
}