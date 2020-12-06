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
        protected virtual IconDescription TransformIcon(IconDescription iconDescription) => iconDescription;

        public override IconDescription GetIcon(T model)
        {
            IconDescription result = _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedIcon(model);
        }

        protected virtual IconDescription GetNotTargetedIcon(T model) => IconDescription.None;
    }

    public abstract class TypeReTargetingDescriptorModuleBase<T, TTarget> : ReTargetingDescriptorModuleBase<T, TTarget>, ITypeIconDescriptorModule<T>
    {
        private readonly ITypeIconDescriptorModule<TTarget>[] _typeModules;

        protected TypeReTargetingDescriptorModuleBase(IIconDescriptorModule<TTarget>[] modules, ITypeIconDescriptorModule<TTarget>[] typeModules)
            : base(modules)
        {
            _typeModules = typeModules;
        }

        protected override sealed IconDescription GetNotTargetedIcon(T model) => GetTypeIcon(model?.GetType());

        public virtual bool Handle(Type type) => type.Is<T>();

        protected abstract Type GetTypeTarget(Type type);
        public IconDescription GetTypeIcon(Type type)
        {
            IconDescription result = _typeModules.Select(x => x.GetTypeIcon(GetTypeTarget(type))).FirstOrDefault(x => x.Defined);
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedTypeIcon(type);
        }

        public override IconDescription GetIcon(T model)
        {
            IconDescription result = base.GetIcon(model);
            if (result.Defined)
                return result;

            return GetTypeIcon(model?.GetType());
        }

        protected virtual IconDescription GetNotTargetedTypeIcon(Type type) => IconDescription.None;
    }
}