using Simulacra.Binding;
using Simulacra.Injection.Binding;

namespace Calame.Demo.Data
{
    public class CircleData : ShapeDataBase<CircleData, CircleObject>
    {
        public float Radius { get; set; } = 50;
        public int Sampling { get; set; } = 64;

        static CircleData()
        {
            PropertyBindings.From(x => x.Radius).To(x => x.Radius);
            PropertyBindings.From(x => x.Sampling).To(x => x.Sampling);
        }
    }
}