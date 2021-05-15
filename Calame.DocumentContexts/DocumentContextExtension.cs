using System.Windows.Input;
using Caliburn.Micro;
using Glyph;

namespace Calame.DocumentContexts
{
    static public class DocumentContextExtension
    {
        static public ISelectionContext<T> GetSelectionContext<T>(this IDocumentContext documentContext)
            where T : class, INotifyDisposed
        {
            return documentContext.TryGetContext<ISelectionContext<T>>()
                ?? new DefaultSelectionContext<T>(documentContext, IoC.Get<IEventAggregator>());
        }

        static public ICommand GetSelectionCommand(this ISelectionContext selectionContext)
        {
            return new SelectionCommand(selectionContext);
        }

        static public ICommand GetSelectionCommand<T>(this ISelectionContext<T> selectionContext)
            where T : class, INotifyDisposed
        {
            return new SelectionCommand<T>(selectionContext);
        }
    }
}