namespace Calame.Icons
{
    public interface IDefaultIconDescriptorModule<in T> : IIconDescriptorModule<T>
    {
        IconDescription GetDefaultIcon(T model);
    }
}