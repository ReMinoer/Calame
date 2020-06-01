namespace Calame.Icons
{
    public interface IDefaultIconDescriptorModule
    {
        IconDescription GetDefaultIcon(object model);
    }

    public interface IDefaultIconDescriptorModule<in T> : IDefaultIconDescriptorModule
    {
        IconDescription GetDefaultIcon(T model);
    }
}