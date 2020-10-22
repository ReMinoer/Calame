using System.Collections.Generic;
using Glyph;

namespace Calame
{
    public interface ISelectionMessage<out T> : IDocumentMessage, INotifyDisposed
    {
        T Item { get; }
        IEnumerable<T> Items { get; }
    }

    public interface ISelectionRequest<out T> : ISelectionMessage<T>
    {
        ISelectionSpread<T> Promoted { get; }
    }

    public interface ISelectionSpread<out T> : ISelectionMessage<T>
    {
    }
}