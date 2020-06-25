﻿namespace Calame.Icons.Base
{
    public abstract class IconDescriptorModuleBase<T> : IIconDescriptorModule<T>
    {
        public abstract IconDescription GetIcon(T model);
        public virtual IconDescription GetIcon(object model) => model is T obj ? GetIcon(obj) : IconDescription.None;
    }
}