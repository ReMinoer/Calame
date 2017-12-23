using System;
using Calame.PropertyGrid.UserControls.Base;
using Microsoft.Xna.Framework;
using Xceed.Wpf.Toolkit;

namespace Calame.PropertyGrid.UserControls
{
    public class Vector2Control : VectorControlBase<SingleUpDown, Vector2?, float?>
    {
        public Vector2Control()
            : base(2)
        {
        }
        
        protected override void UpdateVector(ref Vector2? vector, SingleUpDown[] controls) => vector = new Vector2(controls[0].Value ?? 0, controls[1].Value ?? 0);

        protected override float? GetComponent(Vector2? vector, int index)
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