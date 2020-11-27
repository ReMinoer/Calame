using System;

namespace Calame.Icons
{
    public interface IBaseTypeIconDescriptorModule : IInstanceHandler, ITypeHandler
    {
        IconDescription GetBaseTypeIcon(object model);
        IconDescription GetBaseTypeIcon(Type type);
    }
}