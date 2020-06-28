using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using Diese.Collections;
using Diese.Collections.ReadOnly;

namespace Calame
{
    public class SelectionHistory : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;

        // History will contain only non-empty selection, except for last item if it's the current selection.
        private readonly List<ISelectionSpread<object>> _history;
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
            
            _history = new List<ISelectionSpread<object>>();
            History = new ReadOnlyList<ISelectionSpread<object>>(_history);
        }

        public async Task SelectPreviousAsync()
        {
            if (!HasPrevious)
                return;

            // If current selection is null item, clean it from history before restore previous.
            if (CurrentSelection != null && CurrentSelection.Item == null)
                _history.RemoveAt(CurrentIndex);

            CurrentIndex--;
            await _eventAggregator.PublishAsync(_history[CurrentIndex]);
            OnCurrentIndexChanged();
        }

        public async Task SelectNextAsync()
        {
            if (!HasNext)
                return;

            CurrentIndex++;
            await _eventAggregator.PublishAsync(_history[CurrentIndex]);
            OnCurrentIndexChanged();
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
                    _history.RemoveRange(CurrentIndex, _history.Count - CurrentIndex);
                _history.Add(message);
            }

            OnCurrentIndexChanged();
        }

        private void OnCurrentIndexChanged()
        {
            bool hasPrevious = HasPrevious;
            bool hasNext = HasNext;

            HasPrevious = CurrentIndex > 0 && _history.Count >= 1;
            HasNext = CurrentIndex < _history.Count - 1 && _history.Count >= 1;

            NotifyOfPropertyChange(nameof(CurrentIndex));
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