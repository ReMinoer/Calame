using System;

namespace Calame.Icons
{
    public interface IIconDescriptor
    {
        IconDescription GetIcon(object model);
        IconDescription GetTypeIcon(Type type);
    }

    public interface IIconDescriptor<in T> : IIconDescriptor
    {
        IconDescription GetIcon(T model);
    }
}