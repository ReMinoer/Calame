using System.Collections.Generic;

namespace Calame
{
    public interface ISelection<out T>
    {
        T Item { get; }
        IEnumerable<T> Items { get; }
    }
}