using System;
using System.ComponentModel;
using System.Linq;
using Calame.DataModelViewer.ViewModels;
using Caliburn.Micro;
using Gemini.Modules.UndoRedo;
using Gemini.Modules.UndoRedo.Services;
using Glyph.Tools.UndoRedo;

namespace Calame.DataModelViewer
{
    public class DataModelUndoRedoManager : IUndoRedoManager, IUndoRedoStack, IDisposable
    {
        private readonly UndoRedoManager _base;
        private readonly DataModelViewerViewModel _document;

        public DataModelUndoRedoManager(DataModelViewerViewModel document)
        {
            _base = new UndoRedoManager();
            _base.PropertyChanged += OnBasePropertyChanged;
            _base.BatchBegin += OnBaseBatchBegin;
            _base.BatchEnd += OnBaseBatchEnd;

            _document = document;
        }

        public void Dispose()
        {
            _base.BatchEnd -= OnBaseBatchEnd;
            _base.BatchBegin -= OnBaseBatchBegin;
            _base.PropertyChanged -= OnBasePropertyChanged;
            _base.Dispose();
        }

        private void OnBasePropertyChanged(object s, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        private void OnBaseBatchBegin(object s, EventArgs e) => BatchBegin?.Invoke(this, e);
        private void OnBaseBatchEnd(object s, EventArgs e) => BatchEnd?.Invoke(this, e);

        public IObservableCollection<IUndoableAction> ActionStack => _base.ActionStack;
        public IUndoableAction CurrentAction => _base.CurrentAction;
        public int UndoActionCount => _base.UndoActionCount;
        public int RedoActionCount => _base.RedoActionCount;
        public bool CanUndo => _base.CanUndo;
        public bool CanRedo => _base.CanRedo;

        public int? UndoCountLimit
        {
            get => _base.UndoCountLimit;
            set => _base.UndoCountLimit = value;
        }
        
        public bool IsCurrentlyUndoRedo { get; private set; }

        public event EventHandler BatchBegin;
        public event EventHandler BatchEnd;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Undo(int actionCount)
        {
            IsCurrentlyUndoRedo = true;
            try
            {
                _base.Undo(actionCount);
            }
            finally
            {
                IsCurrentlyUndoRedo = false;
            }
        }

        public void UndoTo(IUndoableAction action)
        {
            IsCurrentlyUndoRedo = true;
            try
            {
                _base.UndoTo(action);
            }
            finally
            {
                IsCurrentlyUndoRedo = false;
            }
        }

        public void UndoAll()
        {
            IsCurrentlyUndoRedo = true;
            try
            {
                _base.UndoAll();
            }
            finally
            {
                IsCurrentlyUndoRedo = false;
            }
        }

        public void Redo(int actionCount)
        {
            IsCurrentlyUndoRedo = true;
            try
            {
                _base.Redo(actionCount);
            }
            finally
            {
                IsCurrentlyUndoRedo = false;
            }
        }

        public void RedoTo(IUndoableAction action)
        {
            IsCurrentlyUndoRedo = true;
            try
            {
                _base.RedoTo(action);
            }
            finally
            {
                IsCurrentlyUndoRedo = false;
            }
        }
        
        public void Push(IUndoRedo undoRedo) => PushAction(new UndoRedoUndoableAction(undoRedo));
        private void PushAction(IUndoableAction action)
        {
            if (IsCurrentlyUndoRedo)
                throw new InvalidOperationException("Actions should not be pushed on stack during an undo or redo operation.");

            _base.PushAction(new DocumentUndoableAction(_document, action));
            _document.SetDirty();
        }

        void IUndoRedoManager.PushAction(IUndoableAction action) => PushAction(action);
        void IUndoRedoManager.ExecuteAction(IUndoableAction action)
        {
            action.Execute();
            PushAction(action);
        }

        private class DocumentUndoableAction : IUndoableAction, IDisposable
        {
            private readonly DataModelViewerViewModel _document;
            private readonly object[] _selectedItems;
            private readonly IUndoableAction _undoableAction;
            private readonly IDisposable _disposableAction;

            public string Name => _undoableAction.Name;

            public DocumentUndoableAction(DataModelViewerViewModel document, IUndoableAction undoableAction)
            {
                _document = document;
                _selectedItems = _document.Viewer.LastSelection?.Items.ToArray() ?? Array.Empty<object>();
                _undoableAction = undoableAction;
                _disposableAction = undoableAction as IDisposable;
            }

            public void Execute()
            {
                _document.SelectAsync(_selectedItems).Wait();
                _undoableAction.Execute();
                _document.SetDirty();
            }

            public void Undo()
            {
                _document.SelectAsync(_selectedItems).Wait();
                _undoableAction.Undo();
                _document.SetDirty();
            }

            public void Dispose()
            {
                _disposableAction.Dispose();
            }
        }
    }
}