using System;
using Calame.UserControls.Base;
using Microsoft.Xna.Framework;
using Xceed.Wpf.Toolkit;

namespace Calame.UserControls
{
    public class PointControl : VectorControlBase<IntegerUpDown, Point?, int?>
    {
        public PointControl()
            : base(2)
        {
        }

        protected override void UpdateVector(ref Point? vector, IntegerUpDown[] controls) => vector = new Point(controls[0].Value ?? 0, controls[1].Value ?? 0);

        protected override int? GetComponent(Point? vector, int index)
        {
            if (vector == null)
                return null;

            switch (index)
            {
                case 0: return vector.Value.X;
                case 1: return vector.Value.Y;
                default: throw new ArgumentException();
            }
        }
    }
}