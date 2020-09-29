namespace Calame.Icons.Base
{
    public abstract class HybridIconDescriptorModuleBase<T> : IconDescriptorModuleBase<T>, IDefaultIconDescriptorModule<T>
    {
        public abstract IconDescription GetDefaultIcon(T model);
        public virtual IconDescription GetDefaultIcon(object model) => model is T obj ? GetDefaultIcon(obj) : IconDescription.None;
    }
}