using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Glyph;

namespace Calame.DocumentContexts
{
    public class DefaultSelectionContext<T> : ISelectionContext<T>
        where T : class, INotifyDisposed
    {
        private readonly IDocumentContext _currentDocument;
        private readonly IEventAggregator _eventAggregator;

        public ICommand SelectCommand { get; }
        event EventHandler ISelectionContext.CanSelectChanged { add { } remove { } }

        public DefaultSelectionContext(IDocumentContext currentDocument, IEventAggregator eventAggregator)
        {
            _currentDocument = currentDocument;
            _eventAggregator = eventAggregator;
            SelectCommand = new SelectionCommand(this);
        }

        public bool CanSelect(object obj) => obj is T instance && CanSelect(instance);
        public bool CanSelect(T instance) => true;

        public Task SelectAsync(object obj)
        {
            if (obj is T instance)
                return SelectAsync(instance);
            return Task.CompletedTask;
        }

        public Task SelectAsync(T instance)
        {
            if (CanSelect(instance))
                return _eventAggregator.PublishAsync(new SelectionRequest<T>(_currentDocument, instance));
            return Task.CompletedTask;
        }
    }
}