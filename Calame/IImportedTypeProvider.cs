using System;
using System.Collections.Generic;

namespace Calame
{
    public interface IImportedTypeProvider
    {
        IEnumerable<Type> Types { get; }
    }
}