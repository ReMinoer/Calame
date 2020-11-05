using System;
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

    public abstract class TypeReTargetingDefaultDescriptorModuleBase<T, TTarget> : ReTargetingDefaultDescriptorModuleBase<T, TTarget>, ITypeDefaultIconDescriptorModule<T>
    {
        private readonly ITypeIconDescriptorModule<TTarget>[] _typeModules;
        private readonly ITypeDefaultIconDescriptorModule<TTarget> _typeDefaultModules;

        protected TypeReTargetingDefaultDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules, IDefaultIconDescriptorModule<TTarget> defaultModule,
            ITypeIconDescriptorModule<TTarget>[] typeModules, ITypeDefaultIconDescriptorModule<TTarget> typeDefaultModules)
            : base(modules, defaultModule)
        {
            _typeModules = typeModules;
            _typeDefaultModules = typeDefaultModules;
        }

        protected abstract Type GetTypeTarget(Type type);
        public IconDescription GetTypeDefaultIcon(Type type)
        {
            IconDescription iconDescription = _typeModules.Select(x => x.GetTypeIcon(GetTypeTarget(type))).FirstOrDefault(x => x.Defined);
            if (iconDescription.Defined)
                return iconDescription;

            return _typeDefaultModules.GetTypeDefaultIcon(GetTypeTarget(type));
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