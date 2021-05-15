using System;
using System.Threading.Tasks;

namespace Calame.DocumentContexts
{
    public interface ISelectionContext
    {
        event EventHandler CanSelectChanged;
        bool CanSelect(object instance);
        Task SelectAsync(object instance);
    }

    public interface ISelectionContext<in T> : ISelectionContext
    {
        bool CanSelect(T instance);
        Task SelectAsync(T instance);
    }
}