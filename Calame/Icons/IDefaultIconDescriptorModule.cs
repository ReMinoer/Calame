namespace Calame.Icons
{
    public interface IDefaultIconDescriptorModule : IIconDescriptorModule
    {
        IconDescription GetDefaultIcon(object model);
    }

    public interface IDefaultIconDescriptorModule<in T> : IIconDescriptorModule<T>
    {
        IconDescription GetDefaultIcon(T model);
    }
}