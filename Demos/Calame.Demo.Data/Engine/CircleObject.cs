using Calame.Demo.Data.Engine.Base;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data.Engine
{
    public class CircleObject : ShapeMeshObjectBase
    {
        private readonly EllipseMesh _mesh;

        public override Color Color
        {
            get => _mesh.CenterColors[0];
            set
            {
                _mesh.CenterColors = new []{ value };
                _mesh.BorderColors = new []{ value };
            }
        }

        public float Radius
        {
            get => _mesh.Width;
            set
            {
                _mesh.Height = value;
                _mesh.Width = value;
            }
        }

        public int Sampling
        {
            get => _mesh.Sampling;
            set => _mesh.Sampling = value;
        }

        public CircleObject(GlyphResolveContext context)
            : base(context)
        {
            _mesh = new EllipseMesh(Color.White, Vector2.Zero, 50);
            MeshRenderer.MeshProviders.Add(_mesh);
        }
    }
}