using System.Linq;

namespace Calame.Icons.Base
{
    public abstract class ReTargetingDefaultDescriptorModuleBase<T, TTarget> : DefaultIconDescriptorModuleBase<T>
    {
        private readonly IIconDescriptorModule<TTarget>[] _modules;
        private readonly IDefaultIconDescriptorModule<TTarget> _defaultModule;

        protected ReTargetingDefaultDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules, IDefaultIconDescriptorModule<TTarget> defaultModule)
        {
            _modules = modules;
            _defaultModule = defaultModule;
        }

        protected abstract TTarget GetTarget(T model);
        public override IconDescription GetDefaultIcon(T model)
        {
            IconDescription iconDescription = _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
            if (iconDescription.Defined)
                return iconDescription;

            return _defaultModule.GetDefaultIcon(GetTarget(model));
        }
    }
}