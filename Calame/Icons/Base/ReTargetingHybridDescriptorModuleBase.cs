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
        protected virtual IconDescription TransformIcon(IconDescription iconDescription) => iconDescription;

        public override sealed IconDescription GetIcon(T model)
        {
            IconDescription result = _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedIcon(model);
        }

        public override sealed IconDescription GetDefaultIcon(T model)
        {
            IconDescription result = _defaultModule.GetDefaultIcon(GetTarget(model));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedDefaultIcon(model);
        }

        protected virtual IconDescription GetNotTargetedIcon(T model) => IconDescription.None;
        protected virtual IconDescription GetNotTargetedDefaultIcon(T model) => IconDescription.None;
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

        protected override sealed IconDescription GetNotTargetedIcon(T model) => GetTypeIcon(model?.GetType());
        protected override sealed IconDescription GetNotTargetedDefaultIcon(T model) => GetTypeDefaultIcon(model?.GetType());

        public virtual bool Handle(Type type) => type.Is<T>();

        protected abstract Type GetTypeTarget(Type type);

        public IconDescription GetTypeIcon(Type type)
        {
            IconDescription result = _typeModules.Select(x => x.GetTypeIcon(GetTypeTarget(type))).FirstOrDefault(x => x.Defined);
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedTypeIcon(type);
        }

        public IconDescription GetTypeDefaultIcon(Type type)
        {
            IconDescription result = _typeDefaultModules.GetTypeDefaultIcon(GetTypeTarget(type));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedTypeDefaultIcon(type);
        }

        protected virtual IconDescription GetNotTargetedTypeIcon(Type type) => IconDescription.None;
        protected virtual IconDescription GetNotTargetedTypeDefaultIcon(Type type) => IconDescription.None;
    }
}