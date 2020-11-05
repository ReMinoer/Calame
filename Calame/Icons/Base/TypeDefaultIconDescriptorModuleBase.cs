using System;

namespace Calame.Icons.Base
{
    public abstract class TypeDefaultIconDescriptorModuleBase : IDefaultIconDescriptorModule, ITypeDefaultIconDescriptorModule
    {
        public abstract IconDescription GetTypeDefaultIcon(Type type);
        public IconDescription GetDefaultIcon(object model) => model is Type type ? GetTypeDefaultIcon(type) : GetTypeDefaultIcon(model?.GetType());
    }

    public abstract class TypeDefaultIconDescriptorModuleBase<T> : DefaultIconDescriptorModuleBase<T>, ITypeDefaultIconDescriptorModule<T>
    {
        public IconDescription GetTypeDefaultIcon(Type type) => GetDefaultTypeIcon(type);
        public override sealed IconDescription GetDefaultIcon(T model) => GetDefaultTypeIcon(model?.GetType());
        protected abstract IconDescription GetDefaultTypeIcon(Type type);
    }
}