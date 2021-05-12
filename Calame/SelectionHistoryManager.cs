using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame
{
    [Export(typeof(SelectionHistoryManager))]
    public class SelectionHistoryManager : PropertyChangedBase, IHandle<ISelectionSpread<object>>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IShell _shell;

        private readonly ConcurrentDictionary<IDocument, SelectionHistory> _documentHistories;
        public ReadOnlyDictionary<IDocument, SelectionHistory> DocumentHistories { get; }

        public SelectionHistory CurrentDocumentHistory => GetHistory(_shell.ActiveItem);

        [ImportingConstructor]
        public SelectionHistoryManager(IShell shell, IEventAggregator eventAggregator)
        {
            _shell = shell;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _documentHistories = new ConcurrentDictionary<IDocument, SelectionHistory>();
            DocumentHistories = new ReadOnlyDictionary<IDocument, SelectionHistory>(_documentHistories);
        }

        public SelectionHistory GetHistory(IDocument document)
        {
            return _documentHistories.GetOrAdd(document, _ => new SelectionHistory(_eventAggregator));
        }

        public void RemoveHistory(IDocument document)
        {
            _documentHistories.TryRemove(document, out _);
        }

        public Task HandleAsync(ISelectionSpread<object> message, CancellationToken cancellationToken)
        {
            GetHistory(message.DocumentContext.Document).AddNewSelection(message);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}