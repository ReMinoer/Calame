using System;

namespace Calame.Icons
{
    public interface ITypeHandler
    {
        bool Handle(Type type);
    }
}