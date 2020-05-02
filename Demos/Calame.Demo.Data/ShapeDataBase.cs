using Glyph.Composition.Modelization;
using Glyph.Tools.Transforming;
using Microsoft.Xna.Framework;
using Simulacra.Binding;
using Simulacra.Injection.Binding;

namespace Calame.Demo.Data
{
    public class ShapeDataBase<TData, T> : BindedData<TData, T>, IShapeData<T>, IPositionController
        where TData : ShapeDataBase<TData, T>
        where T : PrimitiveObjectBase
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; } = Color.White;

        static ShapeDataBase()
        {
            PropertyBindings.From(x => x.Position).To(x => x.Position);
            PropertyBindings.From(x => x.Color).To(x => x.Color);
        }

        Vector2 IPositionController.Position
        {
            get => Position;
            set => Position = value;
        }
    }
}