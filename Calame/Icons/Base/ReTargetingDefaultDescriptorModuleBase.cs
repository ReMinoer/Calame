namespace Calame.Icons.Base
{
    public abstract class ReTargetingDefaultDescriptorModuleBase<T, TTarget> : ReTargetingDescriptorModuleBase<T, TTarget>, IDefaultIconDescriptorModule<T>
    {
        private readonly IDefaultIconDescriptorModule<TTarget> _defaultModule;

        protected ReTargetingDefaultDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules, IDefaultIconDescriptorModule<TTarget> defaultModule)
            : base(modules)
        {
            _defaultModule = defaultModule;
        }

        public IconDescription GetDefaultIcon(T model) => _defaultModule.GetDefaultIcon(GetTarget(model));
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