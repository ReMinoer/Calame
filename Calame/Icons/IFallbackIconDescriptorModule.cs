using System;

namespace Calame.Icons
{
    public interface IFallbackIconDescriptorModule : IInstanceHandler, ITypeHandler
    {
        IconDescription GetBaseTypeIcon(object model);
        IconDescription GetBaseTypeIcon(Type type);
    }
}