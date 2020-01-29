namespace Calame.Icons
{
    public interface IIconDescriptorModule
    {
    }

    public interface IIconDescriptorModule<in T> : IIconDescriptorModule
    {
        IconDescription GetIcon(T model);
    }
}