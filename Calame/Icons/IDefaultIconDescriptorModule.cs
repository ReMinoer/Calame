namespace Calame.Icons
{
    public interface IDefaultIconDescriptorModule : IInstanceHandler
    {
        IconDescription GetDefaultIcon(object model);
    }

    public interface IDefaultIconDescriptorModule<in T> : IDefaultIconDescriptorModule
    {
        IconDescription GetDefaultIcon(T model);
    }
}