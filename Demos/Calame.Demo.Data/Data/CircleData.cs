using System.ComponentModel.DataAnnotations;
using Calame.Demo.Data.Data.Base;
using Calame.Demo.Data.Engine;
using Simulacra.Binding;
using Simulacra.Injection.Binding;

namespace Calame.Demo.Data.Data
{
    public class CircleData : ShapeDataBase<CircleData, CircleObject>
    {
        [Range(0, float.MaxValue)]
        public float Radius { get; set; } = 50;

        [Range(1, float.MaxValue)]
        public int Sampling { get; set; } = 64;

        static CircleData()
        {
            Bindings.From(x => x.Radius).To(x => x.Radius);
            Bindings.From(x => x.Sampling).To(x => x.Sampling);
        }
    }
}