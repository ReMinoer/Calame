namespace Calame.Icons
{
    public interface IIconDescriptor
    {
        IconDescription GetIcon(object model);
    }

    public interface IIconDescriptor<in T> : IIconDescriptor
    {
        IconDescription GetIcon(T model);
    }
}