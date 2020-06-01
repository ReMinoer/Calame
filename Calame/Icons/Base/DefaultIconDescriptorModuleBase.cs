namespace Calame.Icons.Base
{
    public abstract class DefaultIconDescriptorModuleBase<T> : IconDescriptorModuleBase<T>, IDefaultIconDescriptorModule<T>
    {
        public abstract IconDescription GetDefaultIcon(T model);
        public virtual IconDescription GetDefaultIcon(object model) => model is T obj ? GetDefaultIcon(obj) : IconDescription.None;

        public override IconDescription GetIcon(object model)
        {
            if (!(model is T obj))
                return IconDescription.None;

            IconDescription icon = GetIcon(obj);
            return icon.Defined ? icon : GetDefaultIcon(obj);
        }
    }
}