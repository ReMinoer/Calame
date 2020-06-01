using System.Linq;

namespace Calame.Icons.Base
{
    public abstract class ReTargetingDescriptorModuleBase<T, TTarget> : IconDescriptorModuleBase<T>
    {
        private readonly IIconDescriptorModule<TTarget>[] _modules;

        protected ReTargetingDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules)
        {
            _modules = modules;
        }

        protected abstract TTarget GetTarget(T model);
        public override IconDescription GetIcon(T model) => _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
    }
}