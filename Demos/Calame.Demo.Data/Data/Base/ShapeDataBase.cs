using Calame.Demo.Data.Engine.Base;
using Glyph.Composition.Modelization;
using Microsoft.Xna.Framework;
using Simulacra.Binding;
using Simulacra.Injection.Binding;

namespace Calame.Demo.Data.Data.Base
{
    public abstract class ShapeDataBase<TData, T> : BindedData<TData, T>, IShapeData<T>
        where TData : ShapeDataBase<TData, T>
        where T : PrimitiveObjectBase
    {
        public Color Color { get; set; } = Color.White;

        static ShapeDataBase()
        {
            Bindings.From(x => x.Color).To(x => x.Color);
        }
    }
}