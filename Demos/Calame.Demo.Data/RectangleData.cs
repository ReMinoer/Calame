using Simulacra.Injection.Binding;

namespace Calame.Demo.Data
{
    public class RectangleData : ShapeDataBase<RectangleData, RectangleObject>
    {
        public float Width { get; set; } = 100;
        public float Height { get; set; } = 100;

        static RectangleData()
        {
            PropertyBindings.AddProperty(x => x.Width, x => x.Width);
            PropertyBindings.AddProperty(x => x.Height, x => x.Height);
        }
    }
}