namespace Calame.Icons.Base
{
    public abstract class IconDescriptorModuleBase<T> : IIconDescriptorModule<T>
    {
        public abstract IconDescription GetIcon(T model);
        IconDescription IIconDescriptorModule.GetIcon(object model) => model is T obj ? GetIcon(obj) : IconDescription.None;
    }
}