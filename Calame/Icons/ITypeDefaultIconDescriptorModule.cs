using System;

namespace Calame.Icons
{
    public interface ITypeDefaultIconDescriptorModule
    {
        IconDescription GetTypeDefaultIcon(Type type);
    }

    public interface ITypeDefaultIconDescriptorModule<T> : ITypeDefaultIconDescriptorModule
    {
    }
}