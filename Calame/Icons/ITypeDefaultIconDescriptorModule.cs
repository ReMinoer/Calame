using System;

namespace Calame.Icons
{
    public interface ITypeDefaultIconDescriptorModule : ITypeHandler
    {
        IconDescription GetTypeDefaultIcon(Type type);
    }

    public interface ITypeDefaultIconDescriptorModule<T> : ITypeDefaultIconDescriptorModule
    {
    }
}