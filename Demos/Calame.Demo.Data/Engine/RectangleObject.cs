using Calame.Demo.Data.Engine.Base;
using Glyph.Core;
using Glyph.Graphics.Primitives;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data.Engine
{
    public class RectangleObject : PrimitiveObjectBase
    {
        private CenteredRectangle _rectangle;
        private readonly TriangulableShapePrimitive<CenteredRectangle> _primitive;

        public override Color Color
        {
            get => _primitive.Color;
            set => _primitive.Color = value;
        }

        public float Width
        {
            get => _rectangle.Width;
            set
            {
                _rectangle.Width = value;
                RefreshPrimitive();
            }
        }

        public float Height
        {
            get => _rectangle.Height;
            set
            {
                _rectangle.Height = value;
                RefreshPrimitive();
            }
        }

        public RectangleObject(GlyphResolveContext context)
            : base(context)
        {
            _rectangle = new CenteredRectangle(Vector2.Zero, new Vector2(100, 100));
            _primitive = _rectangle.ToPrimitive(Color.White);
            PrimitiveRenderer.PrimitiveProviders.Add(_primitive);
        }

        private void RefreshPrimitive()
        {
            _primitive.Shape = _rectangle;
        }
    }
}