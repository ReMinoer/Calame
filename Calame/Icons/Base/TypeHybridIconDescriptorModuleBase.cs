using System;

namespace Calame.Icons.Base
{
    public abstract class TypeHybridIconDescriptorModuleBase : TypeIconDescriptorModuleBase, IDefaultIconDescriptorModule, ITypeDefaultIconDescriptorModule
    {
        public abstract IconDescription GetTypeDefaultIcon(Type type);
        public IconDescription GetDefaultIcon(object model) => model is Type type ? GetTypeDefaultIcon(type) : GetTypeDefaultIcon(model?.GetType());
    }

    public abstract class TypeHybridIconDescriptorModuleBase<T> : HybridIconDescriptorModuleBase<T>, ITypeIconDescriptorModule<T>, ITypeDefaultIconDescriptorModule<T>
    {
        public abstract IconDescription GetTypeIcon(Type type);
        public override sealed IconDescription GetIcon(T model) => GetTypeIcon(model?.GetType());

        public abstract IconDescription GetTypeDefaultIcon(Type type);
        public override sealed IconDescription GetDefaultIcon(T model) => GetTypeDefaultIcon(model?.GetType());
    }
}