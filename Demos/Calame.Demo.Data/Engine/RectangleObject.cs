using Calame.Demo.Data.Engine.Base;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data.Engine
{
    public class RectangleObject : ShapeMeshObjectBase
    {
        private CenteredRectangle _rectangle;
        private readonly TriangulableShapeMesh<CenteredRectangle> _mesh;

        public override Color Color
        {
            get => _mesh.Color;
            set => _mesh.Color = value;
        }

        public float Width
        {
            get => _rectangle.Width;
            set
            {
                _rectangle.Width = value;
                RefreshMesh();
            }
        }

        public float Height
        {
            get => _rectangle.Height;
            set
            {
                _rectangle.Height = value;
                RefreshMesh();
            }
        }

        public RectangleObject(GlyphResolveContext context)
            : base(context)
        {
            _rectangle = new CenteredRectangle(Vector2.Zero, new Vector2(100, 100));
            _mesh = _rectangle.ToMesh(Color.White);
            MeshRenderer.MeshProviders.Add(_mesh);
        }

        private void RefreshMesh()
        {
            _mesh.Shape = _rectangle;
        }
    }
}