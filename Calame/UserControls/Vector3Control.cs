using System;
using Calame.UserControls.Base;
using Microsoft.Xna.Framework;
using Xceed.Wpf.Toolkit;

namespace Calame.UserControls
{
    public class Vector3Control : VectorControlBase<SingleUpDown, Vector3?, float?>
    {
        public Vector3Control()
            : base(3)
        {
        }

        protected override Vector3? UpdateVector(Vector3? vector, SingleUpDown[] controls) => new Vector3(controls[0].Value ?? 0, controls[1].Value ?? 0, controls[2].Value ?? 0);

        protected override float? GetComponent(Vector3? vector, int index)
        {
            if (vector == null)
                return null;

            switch (index)
            {
                case 0: return vector.Value.X;
                case 1: return vector.Value.Y;
                case 2: return vector.Value.Z;
                default: throw new ArgumentException();
            }
        }
    }
}