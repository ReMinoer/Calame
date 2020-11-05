using System;

namespace Calame.Icons.Base
{
    public abstract class TypeIconDescriptorModuleBase : IIconDescriptorModule, ITypeIconDescriptorModule
    {
        public abstract IconDescription GetTypeIcon(Type type);
        public IconDescription GetIcon(object model) => model is Type type ? GetTypeIcon(type) : GetTypeIcon(model?.GetType());
    }

    public abstract class TypeIconDescriptorModuleBase<T> : IconDescriptorModuleBase<T>, ITypeIconDescriptorModule<T>
    {
        public abstract IconDescription GetTypeIcon(Type type);
        public override sealed IconDescription GetIcon(T model) => GetTypeIcon(model?.GetType());
    }
}