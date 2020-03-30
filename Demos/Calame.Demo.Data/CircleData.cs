using Simulacra.Injection.Binding;

namespace Calame.Demo.Data
{
    public class CircleData : ShapeDataBase<CircleData, CircleObject>
    {
        public float Radius { get; set; } = 50;
        public int Sampling { get; set; } = 64;

        static CircleData()
        {
            PropertyBindings.AddProperty(x => x.Radius, x => x.Radius);
            PropertyBindings.AddProperty(x => x.Sampling, x => x.Sampling);
        }
    }
}