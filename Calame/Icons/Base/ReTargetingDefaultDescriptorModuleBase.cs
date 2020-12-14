using System;
using Caliburn.Micro;
using Diese;

namespace Calame.Icons.Base
{
    public abstract class ReTargetingDefaultDescriptorModuleBase<T, TTarget> : DefaultIconDescriptorModuleBase<T>
    {
        public override IconDescription GetDefaultIcon(T model)
        {
            IconDescription result = ReTargetIconDescriptorManager.Instance.GetDescriptor().GetIcon(GetTarget(model));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedDefaultIcon(model);
        }

        protected abstract TTarget GetTarget(T model);
        protected virtual IconDescription TransformIcon(IconDescription iconDescription) => iconDescription;
        protected virtual IconDescription GetNotTargetedDefaultIcon(T model) => IconDescription.None;
    }

    public abstract class TypeReTargetingDefaultDescriptorModuleBase<T, TTarget> : ReTargetingDefaultDescriptorModuleBase<T, TTarget>, ITypeDefaultIconDescriptorModule<T>
    {
        public virtual bool Handle(Type type) => type.Is<T>();

        public override IconDescription GetDefaultIcon(T model)
        {
            IconDescription result = base.GetDefaultIcon(model);
            if (result.Defined)
                return TransformIcon(result);

            return GetTypeDefaultIcon(model?.GetType());
        }

        public IconDescription GetTypeDefaultIcon(Type type)
        {
            IconDescription result = ReTargetIconDescriptorManager.Instance.GetDescriptor().GetTypeIcon(GetTypeTarget(type));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedTypeDefaultIcon(type);
        }

        protected abstract Type GetTypeTarget(Type type);
        protected virtual IconDescription GetNotTargetedTypeDefaultIcon(Type type) => IconDescription.None;
    }
}