using System;

namespace Calame.Icons
{
    public interface ITypeIconDescriptorModule
    {
        IconDescription GetTypeIcon(Type type);
    }

    public interface ITypeIconDescriptorModule<T> : ITypeIconDescriptorModule
    {
    }
}