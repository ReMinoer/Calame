using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Calame
{
    [Export(typeof(SelectionHistoryManager))]
    public class SelectionHistoryManager : PropertyChangedBase, IHandle<IDocumentContext>, IHandle<ISelectionSpread<object>>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ConcurrentDictionary<IDocumentContext, SelectionHistory> _documentHistories;
        public ReadOnlyDictionary<IDocumentContext, SelectionHistory> DocumentHistories { get; }

        private SelectionHistory _currentDocumentHistory;
        public SelectionHistory CurrentDocumentHistory
        {
            get => _currentDocumentHistory;
            private set
            {
                if (Set(ref _currentDocumentHistory, value))
                    CurrentDocumentHistoryChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private IDocumentContext CurrentDocument
        {
            set => CurrentDocumentHistory = _documentHistories.GetOrAdd(value, _ => new SelectionHistory(_eventAggregator));
        }

        public event EventHandler CurrentDocumentHistoryChanged;

        [ImportingConstructor]
        public SelectionHistoryManager(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _documentHistories = new ConcurrentDictionary<IDocumentContext, SelectionHistory>();
            DocumentHistories = new ReadOnlyDictionary<IDocumentContext, SelectionHistory>(_documentHistories);
        }

        public Task HandleAsync(IDocumentContext message, CancellationToken cancellationToken)
        {
            CurrentDocument = message;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ISelectionSpread<object> message, CancellationToken cancellationToken)
        {
            CurrentDocument = message.DocumentContext;
            CurrentDocumentHistory.AddNewSelection(message);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}