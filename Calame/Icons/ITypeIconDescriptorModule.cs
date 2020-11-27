using System;

namespace Calame.Icons
{
    public interface ITypeIconDescriptorModule : ITypeHandler
    {
        IconDescription GetTypeIcon(Type type);
    }

    public interface ITypeIconDescriptorModule<T> : ITypeIconDescriptorModule
    {
    }
}