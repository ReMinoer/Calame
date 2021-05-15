using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calame.DocumentContexts
{
    public interface ISelectionCommandContext
    {
        ICommand SelectCommand { get; }
        event EventHandler CanSelectChanged;
        bool CanSelect(object instance);
        Task SelectAsync(object instance);
    }

    public interface ISelectionCommandContext<in T> : ISelectionCommandContext
    {
        bool CanSelect(T instance);
        Task SelectAsync(T instance);
    }
}