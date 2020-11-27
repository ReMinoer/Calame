using System;
using System.Linq;
using Diese;

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

        public override bool Handle(object model) => model is T;

        protected abstract TTarget GetTarget(T model);
        public override IconDescription GetIcon(T model) => _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
        public override IconDescription GetDefaultIcon(T model) => _defaultModule.GetDefaultIcon(GetTarget(model));
    }

    public abstract class TypeReTargetingHybridDescriptorModuleBase<T, TTarget> : ReTargetingHybridDescriptorModuleBase<T, TTarget>, ITypeIconDescriptorModule<T>, ITypeDefaultIconDescriptorModule<T>
    {
        private readonly ITypeIconDescriptorModule<TTarget>[] _typeModules;
        private readonly ITypeDefaultIconDescriptorModule<TTarget> _typeDefaultModules;

        protected TypeReTargetingHybridDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules, IDefaultIconDescriptorModule<TTarget> defaultModule,
            ITypeIconDescriptorModule<TTarget>[] typeModules, ITypeDefaultIconDescriptorModule<TTarget> typeDefaultModules)
            : base(modules, defaultModule)
        {
            _typeModules = typeModules;
            _typeDefaultModules = typeDefaultModules;
        }

        public virtual bool Handle(Type type) => type.Is<T>();

        protected abstract Type GetTypeTarget(Type type);
        public IconDescription GetTypeIcon(Type type) => _typeModules.Select(x => x.GetTypeIcon(GetTypeTarget(type))).FirstOrDefault(x => x.Defined);
        public IconDescription GetTypeDefaultIcon(Type type) => _typeDefaultModules.GetTypeDefaultIcon(GetTypeTarget(type));

        public override IconDescription GetIcon(T model)
        {
            IconDescription result = base.GetIcon(model);
            if (result.Defined)
                return result;

            return GetTypeIcon(model?.GetType());
        }

        public override IconDescription GetDefaultIcon(T model)
        {
            IconDescription result = base.GetDefaultIcon(model);
            if (result.Defined)
                return result;

            return GetTypeDefaultIcon(model?.GetType());
        }
    }
}