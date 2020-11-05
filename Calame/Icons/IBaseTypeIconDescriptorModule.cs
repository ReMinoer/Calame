using System;

namespace Calame.Icons
{
    public interface IBaseTypeIconDescriptorModule
    {
        IconDescription GetBaseTypeIcon(object model);
        IconDescription GetBaseTypeIcon(Type type);
    }
}