using System;
using System.Linq;
using Diese;

namespace Calame.Icons.Base
{
    public abstract class ReTargetingDescriptorModuleBase<T, TTarget> : IconDescriptorModuleBase<T>
    {
        private readonly IIconDescriptorModule<TTarget>[] _modules;

        protected ReTargetingDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules)
        {
            _modules = modules;
        }

        public override bool Handle(object model) => model is T;

        protected abstract TTarget GetTarget(T model);
        public override IconDescription GetIcon(T model) => _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
    }

    public abstract class TypeReTargetingDescriptorModuleBase<T, TTarget> : ReTargetingDescriptorModuleBase<T, TTarget>, ITypeIconDescriptorModule<T>
    {
        private readonly ITypeIconDescriptorModule<TTarget>[] _typeModules;

        protected TypeReTargetingDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules, ITypeIconDescriptorModule<TTarget>[] typeModules)
            : base(modules)
        {
            _typeModules = typeModules;
        }

        public virtual bool Handle(Type type) => type.Is<T>();

        protected abstract Type GetTypeTarget(Type type);
        public IconDescription GetTypeIcon(Type type) => _typeModules.Select(x => x.GetTypeIcon(GetTypeTarget(type))).FirstOrDefault(x => x.Defined);

        public override IconDescription GetIcon(T model)
        {
            IconDescription result = base.GetIcon(model);
            if (result.Defined)
                return result;

            return GetTypeIcon(model?.GetType());
        }
    }
}