namespace Calame.Icons
{
    public interface IIconDescriptorManager
    {
        IIconDescriptor GetDescriptor();
        IIconDescriptor<T> GetDescriptor<T>();
    }
}