using Simulacra.Binding;
using Simulacra.Injection.Binding;

namespace Calame.Demo.Data
{
    public class RectangleData : ShapeDataBase<RectangleData, RectangleObject>
    {
        public float Width { get; set; } = 100;
        public float Height { get; set; } = 100;

        static RectangleData()
        {
            PropertyBindings.From(x => x.Width).To(x => x.Width);
            PropertyBindings.From(x => x.Height).To(x => x.Height);
        }
    }
}