using System;
using Diese;

namespace Calame.Icons.Base
{
    public abstract class ReTargetingDescriptorModuleBase<T, TTarget> : IconDescriptorModuleBase<T>
    {
        public override bool Handle(object model) => model is T;

        public override IconDescription GetIcon(T model)
        {
            IconDescription result = ReTargetIconDescriptorManager.Instance.GetDescriptor().GetIcon(GetTarget(model));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedIcon(model);
        }

        protected abstract TTarget GetTarget(T model);
        protected virtual IconDescription TransformIcon(IconDescription iconDescription) => iconDescription;
        protected virtual IconDescription GetNotTargetedIcon(T model) => IconDescription.None;
    }

    public abstract class TypeReTargetingDescriptorModuleBase : IIconDescriptorModule, ITypeIconDescriptorModule
    {
        public virtual bool Handle(object model) => Handle(model?.GetType());
        public abstract bool Handle(Type type);

        public IconDescription GetIcon(object model) => GetTypeIcon(model?.GetType());
        public IconDescription GetTypeIcon(Type type)
        {
            IconDescription result = ReTargetIconDescriptorManager.Instance.GetDescriptor().GetTypeIcon(GetTypeTarget(type));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedTypeIcon(type);
        }

        protected abstract Type GetTypeTarget(Type type);
        protected virtual IconDescription TransformIcon(IconDescription iconDescription) => iconDescription;
        protected virtual IconDescription GetNotTargetedTypeIcon(Type type) => IconDescription.None;
    }

    public abstract class TypeReTargetingDescriptorModuleBase<T, TTarget> : ReTargetingDescriptorModuleBase<T, TTarget>, ITypeIconDescriptorModule<T>
    {
        public virtual bool Handle(Type type) => type.Is<T>();

        public override sealed IconDescription GetIcon(T model)
        {
            IconDescription result = base.GetIcon(model);
            if (result.Defined)
                return TransformIcon(result);

            return GetTypeIcon(model?.GetType());
        }

        public IconDescription GetTypeIcon(Type type)
        {
            IconDescription result = ReTargetIconDescriptorManager.Instance.GetDescriptor().GetTypeIcon(GetTypeTarget(type));
            if (result.Defined)
                return TransformIcon(result);

            return GetNotTargetedTypeIcon(type);
        }

        protected abstract Type GetTypeTarget(Type type);
        protected virtual IconDescription GetNotTargetedTypeIcon(Type type) => IconDescription.None;
    }
}