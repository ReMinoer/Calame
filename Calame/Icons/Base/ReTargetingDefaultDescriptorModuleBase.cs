using System;
using System.Linq;
using Caliburn.Micro;
using Diese;

namespace Calame.Icons.Base
{
    public abstract class ReTargetingDefaultDescriptorModuleBase<T, TTarget> : DefaultIconDescriptorModuleBase<T>
    {
        private readonly IIconDescriptorModule<TTarget>[] _modules;
        private readonly IDefaultIconDescriptorModule<TTarget> _defaultModule;

        protected ReTargetingDefaultDescriptorModuleBase()
        {
            _modules = IoC.GetAll<IIconDescriptorModule<TTarget>>().ToArray();
            _defaultModule = IoC.Get<IDefaultIconDescriptorModule<TTarget>>();
        }

        public override bool Handle(object model) => model is T;

        protected abstract TTarget GetTarget(T model);
        protected virtual IconDescription TransformIcon(IconDescription iconDescription) => iconDescription;

        public override sealed IconDescription GetDefaultIcon(T model)
        {
            IconDescription result = _modules.Select(x => x.GetIcon(GetTarget(model))).FirstOrDefault(x => x.Defined);
            if (result.Defined)
                return TransformIcon(result);

            result = _defaultModule.GetDefaultIcon(GetTarget(model));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedDefaultIcon(model);
        }

        protected virtual IconDescription GetNotTargetedDefaultIcon(T model) => IconDescription.None;
    }

    public abstract class TypeReTargetingDefaultDescriptorModuleBase<T, TTarget> : ReTargetingDefaultDescriptorModuleBase<T, TTarget>, ITypeDefaultIconDescriptorModule<T>
    {
        private readonly ITypeIconDescriptorModule<TTarget>[] _typeModules;
        private readonly ITypeDefaultIconDescriptorModule<TTarget> _typeDefaultModules;

        protected TypeReTargetingDefaultDescriptorModuleBase()
        {
            _typeModules = IoC.GetAll<ITypeIconDescriptorModule<TTarget>>().ToArray();
            _typeDefaultModules = IoC.Get<ITypeDefaultIconDescriptorModule<TTarget>>();
        }

        protected override sealed IconDescription GetNotTargetedDefaultIcon(T model) => GetTypeDefaultIcon(model?.GetType());

        public virtual bool Handle(Type type) => type.Is<T>();

        protected abstract Type GetTypeTarget(Type type);
        public IconDescription GetTypeDefaultIcon(Type type)
        {
            IconDescription result = _typeModules.Select(x => x.GetTypeIcon(GetTypeTarget(type))).FirstOrDefault(x => x.Defined);
            if (result.Defined)
                return TransformIcon(result);

            result = _typeDefaultModules.GetTypeDefaultIcon(GetTypeTarget(type));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedTypeDefaultIcon(type);
        }

        protected virtual IconDescription GetNotTargetedTypeDefaultIcon(Type type) => IconDescription.None;
    }
}