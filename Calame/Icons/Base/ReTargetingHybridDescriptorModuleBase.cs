using System.Linq;

namespace Calame.Icons.Base
{
    public abstract class ReTargetingHybridDescriptorModuleBase<T, TTarget> : HybridIconDescriptorModuleBase<T>
    {
        private readonly IIconDescriptorModule<TTarget>[] _modules;
        private readonly IDefaultIconDescriptorModule<TTarget> _defaultModule;

        protected ReTargetingHybridDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules, IDefaultIconDescriptorModule<TTarget> defaultModule)
        {
            _modules = modules;
            _defaultModule = defaultModule;
        }

        protected abstract TTarget GetTarget(T model);
        public override IconDescription GetIcon(T model) => _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
        public override IconDescription GetDefaultIcon(T model) => _defaultModule.GetDefaultIcon(GetTarget(model));
    }
}