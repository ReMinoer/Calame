namespace Calame.Icons
{
    public interface IIconDescriptorModule
    {
    }

    public interface IIconDescriptorModule<in T> : IIconDescriptorModule, IIconDescriptor<T>
    {
    }
}