using System;
using Diese;

namespace Calame.Icons.Base
{
    public abstract class TypeIconDescriptorModuleBase : IIconDescriptorModule, ITypeIconDescriptorModule
    {
        public abstract bool Handle(Type type);
        public virtual bool Handle(object model) => model is Type type ? Handle(type) : Handle(model?.GetType());

        public abstract IconDescription GetTypeIcon(Type type);
        public virtual IconDescription GetIcon(object model) => model is Type type ? GetTypeIcon(type) : GetTypeIcon(model?.GetType());
    }

    public abstract class TypeIconDescriptorModuleBase<T> : IconDescriptorModuleBase<T>, ITypeIconDescriptorModule<T>
    {
        public virtual bool Handle(Type type) => type.Is<T>();
        public override sealed bool Handle(object model) => Handle(model?.GetType());

        public abstract IconDescription GetTypeIcon(Type type);
        public override sealed IconDescription GetIcon(T model) => GetTypeIcon(model?.GetType());
    }
}