namespace Calame.Icons
{
    public interface IIconDescriptorModule
    {
        IconDescription GetIcon(object model);
    }

    public interface IIconDescriptorModule<in T> : IIconDescriptorModule
    {
        IconDescription GetIcon(T model);
    }
}