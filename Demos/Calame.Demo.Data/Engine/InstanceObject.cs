using Glyph.Composition;
using Glyph.Core;

namespace Calame.Demo.Data.Engine
{
    public class InstanceObject : GlyphObject, IInstanceObject
    {
        public SceneNode SceneNode { get; }

        private IGlyphComponent _component;
        public IGlyphComponent Component
        {
            get => _component;
            set => SetPropertyComponent(ref _component, value, disposeOnRemove: true);
        }

        public InstanceObject(GlyphResolveContext context)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
        }
    }
}