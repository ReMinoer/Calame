using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Diese.Collections;
using Diese.Collections.ReadOnly;
using Glyph;

namespace Calame
{
    public class SelectionHistory : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;

        // History will contain only non-empty selection, except for last item if it's the current selection.
        private readonly DisposableTracker<ISelectionSpread<object>> _history;
        public ReadOnlyList<ISelectionSpread<object>> History { get; }

        public int CurrentIndex { get; private set; } = -1;
        public bool HasPrevious { get; private set; }
        public bool HasNext { get; private set; }
        public ISelectionSpread<object> CurrentSelection => CurrentIndex >= 0 ? _history[CurrentIndex] : null;

        public event EventHandler HasPreviousChanged;
        public event EventHandler HasNextChanged;

        public SelectionHistory(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
            _history = new DisposableTracker<ISelectionSpread<object>>();
            _history.ItemDisposed += OnSelectionItemDisposed;

            History = new ReadOnlyList<ISelectionSpread<object>>(_history);
        }

        public async Task SelectPreviousAsync()
        {
            if (!HasPrevious)
                return;

            // If current selection is null item, clean it from history before restore previous.
            if (CurrentSelection != null && CurrentSelection.Item == null)
                _history.UnregisterAt(CurrentIndex);

            CurrentIndex--;
            OnHistoryChanged();
            await _eventAggregator.PublishAsync(_history[CurrentIndex]);
        }

        public async Task SelectNextAsync()
        {
            if (!HasNext)
                return;

            CurrentIndex++;
            OnHistoryChanged();
            await _eventAggregator.PublishAsync(_history[CurrentIndex]);
        }

        public void AddNewSelection(ISelectionSpread<object> message)
        {
            // If new selection is same as current, return directly.
            if (CurrentSelection != null && message.Items.SetEquals(CurrentSelection.Items))
                return;

            // If current selection is null, replace it with new selection.
            if (CurrentSelection != null && CurrentSelection.Item == null)
            {
                _history[CurrentIndex] = message;
            }
            else // Else, remove next selection in history and add new selection instead.
            {
                CurrentIndex++;
                if (_history.Count > CurrentIndex)
                    _history.UnregisterRange(CurrentIndex, _history.Count - CurrentIndex);
                _history.Register(message);
            }

            OnHistoryChanged();
        }

        private void OnSelectionItemDisposed(object sender, ItemDisposedEventArgs e)
        {
            bool currentIndexChanged = false;
            bool currentSelectionChanged = e.Index == CurrentIndex;

            // Move index if disposed selection was before current selection in history or current selection itself.
            if (e.Index <= CurrentIndex)
            {
                CurrentIndex--;
                currentIndexChanged = true;
            }

            // If previous and next of disposed selection (consecutive in history at that point) represent a same other selection, unregister the previous one.
            if (e.Index >= 1 && e.Index < _history.Count && _history[e.Index - 1].Items.SetEquals(_history[e.Index].Items))
            {
                _history.UnregisterAt(e.Index - 1);
                if (e.Index <= CurrentIndex)
                {
                    CurrentIndex--;
                    currentIndexChanged = true;
                }
            }

            // If new index is out of range, it means we don't have previous selection. So we get next one if it exists.
            if (CurrentIndex < 0 && _history.Count > 0)
                CurrentIndex = 0;
                
            OnHistoryChanged(currentIndexChanged, currentSelectionChanged);

            // If disposed item was current selection, restore previous, or next, if it exists.
            if (currentSelectionChanged && CurrentIndex >= 0)
                _eventAggregator.PublishAsync(_history[CurrentIndex]).Wait();
        }

        private void OnHistoryChanged(bool currentIndexChanged = true, bool currentSelectionChanged = true)
        {
            bool hasPrevious = HasPrevious;
            bool hasNext = HasNext;

            HasPrevious = CurrentIndex > 0 && _history.Count >= 1;
            HasNext = CurrentIndex < _history.Count - 1 && _history.Count >= 1;

            if (currentIndexChanged)
                NotifyOfPropertyChange(nameof(CurrentIndex));
            if (currentSelectionChanged)
                NotifyOfPropertyChange(nameof(CurrentSelection));

            if (hasPrevious != HasPrevious)
            {
                NotifyOfPropertyChange(nameof(HasPrevious));
                HasPreviousChanged?.Invoke(this, EventArgs.Empty);
            }
            if (hasNext != HasNext)
            {
                NotifyOfPropertyChange(nameof(HasNext));
                HasNextChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}