namespace Calame.Icons
{
    public interface IIconDescriptor<in T>
    {
        IconDescription GetIcon(T model);
    }
}