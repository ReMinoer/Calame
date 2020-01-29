namespace Calame.Icons
{
    public interface IIconDescriptorManager
    {
        IIconDescriptor<T> GetDescriptor<T>();
    }
}