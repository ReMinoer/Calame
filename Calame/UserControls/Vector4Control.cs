using System;
using Calame.UserControls.Base;
using Microsoft.Xna.Framework;
using Xceed.Wpf.Toolkit;

namespace Calame.UserControls
{
    public class Vector4Control : VectorControlBase<SingleUpDown, Vector4?, float?>
    {
        public Vector4Control()
            : base(4)
        {
        }

        protected override Vector4? UpdateVector(Vector4? vector, SingleUpDown[] controls) => new Vector4(controls[0].Value ?? 0, controls[1].Value ?? 0, controls[2].Value ?? 0, controls[3].Value ?? 0);

        protected override float? GetComponent(Vector4? vector, int index)
        {
            if (vector == null)
                return null;

            switch (index)
            {
                case 0: return vector.Value.X;
                case 1: return vector.Value.Y;
                case 2: return vector.Value.Z;
                case 3: return vector.Value.W;
                default: throw new ArgumentException();
            }
        }
    }
}