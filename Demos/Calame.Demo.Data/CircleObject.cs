using Glyph.Core;
using Glyph.Graphics.Primitives;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Data
{
    public class CircleObject : PrimitiveObjectBase
    {
        private readonly EllipsePrimitive _primitive;

        public override Color Color
        {
            get => _primitive.CenterColors[0];
            set
            {
                _primitive.CenterColors = new []{ value };
                _primitive.BorderColors = new []{ value };
            }
        }

        public float Radius
        {
            get => _primitive.Width;
            set
            {
                _primitive.Height = value;
                _primitive.Width = value;
            }
        }

        public int Sampling
        {
            get => _primitive.Sampling;
            set => _primitive.Sampling = value;
        }

        public CircleObject(GlyphResolveContext context)
            : base(context)
        {
            _primitive = new EllipsePrimitive(Color.White, Vector2.Zero, 50);
            PrimitiveRenderer.PrimitiveProviders.Add(_primitive);
        }
    }
}