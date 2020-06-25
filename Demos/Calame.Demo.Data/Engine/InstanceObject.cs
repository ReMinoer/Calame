using Glyph.Composition;
using Glyph.Core;

namespace Calame.Demo.Data.Engine
{
    public class InstanceObject : GlyphObject
    {
        public SceneNode SceneNode { get; }

        public IGlyphComponent Component
        {
            get => GetComponentProperty();
            set => SetComponentProperty(value);
        }

        public InstanceObject(GlyphResolveContext context)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
        }
    }
}