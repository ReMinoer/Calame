using System;
using Diese;

namespace Calame.Icons.Base
{
    public abstract class TypeDefaultIconDescriptorModuleBase : IDefaultIconDescriptorModule, ITypeDefaultIconDescriptorModule
    {
        public abstract bool Handle(Type type);
        public virtual bool Handle(object model) => model is Type type ? Handle(type) : Handle(model?.GetType());

        public abstract IconDescription GetTypeDefaultIcon(Type type);
        public virtual IconDescription GetDefaultIcon(object model) => model is Type type ? GetTypeDefaultIcon(type) : GetTypeDefaultIcon(model?.GetType());
    }

    public abstract class TypeDefaultIconDescriptorModuleBase<T> : DefaultIconDescriptorModuleBase<T>, ITypeDefaultIconDescriptorModule<T>
    {
        public virtual bool Handle(Type type) => type.Is<T>();
        public override sealed bool Handle(object model) => Handle(model?.GetType());

        public IconDescription GetTypeDefaultIcon(Type type) => GetDefaultTypeIcon(type);
        public override sealed IconDescription GetDefaultIcon(T model) => GetDefaultTypeIcon(model?.GetType());
        protected abstract IconDescription GetDefaultTypeIcon(Type type);
    }
}