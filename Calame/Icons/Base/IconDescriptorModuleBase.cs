namespace Calame.Icons.Base
{
    public abstract class IconDescriptorModuleBase<T> : IIconDescriptorModule<T>
    {
        public virtual bool Handle(object model) => model is T;

        public abstract IconDescription GetIcon(T model);
        IconDescription IIconDescriptorModule.GetIcon(object model) => model is T obj ? GetIcon(obj) : IconDescription.None;
    }
}