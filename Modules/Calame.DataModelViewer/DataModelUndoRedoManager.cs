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
    public class DataModelUndoRedoManager : IUndoRedoManager, IUndoRedoStack
    {
        private readonly UndoRedoManager _base;
        private readonly DataModelViewerViewModel _document;

        public DataModelUndoRedoManager(DataModelViewerViewModel document)
        {
            _base = new UndoRedoManager();
            _base.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, e);
            _base.BatchBegin += (s, e) => BatchBegin?.Invoke(this, e);
            _base.BatchEnd += (s, e) => BatchEnd?.Invoke(this, e);

            _document = document;
        }

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

        public void ExecuteAction(IUndoableAction action)
        {
            if (IsCurrentlyUndoRedo)
                throw new InvalidOperationException("Actions should not be pushed on stack during an undo or redo operation.");

            _base.ExecuteAction(new DocumentUndoableAction(_document, action));
            _document.SetDirty();
        }

        public void PushAction(IUndoableAction action)
        {
            if (IsCurrentlyUndoRedo)
                throw new InvalidOperationException("Actions should not be pushed on stack during an undo or redo operation.");

            _base.PushAction(new DocumentUndoableAction(_document, action));
            _document.SetDirty();
        }

        public bool IsCurrentlyUndoRedo { get; private set; }
        void IUndoRedoStack.Execute(IUndoRedo undoRedo) => ExecuteAction(new UndoRedoUndoableAction(undoRedo));
        void IUndoRedoStack.Push(IUndoRedo undoRedo) => PushAction(new UndoRedoUndoableAction(undoRedo));

        private class DocumentUndoableAction : IUndoableAction
        {
            private readonly DataModelViewerViewModel _document;
            private readonly object[] _selectedItems;
            private readonly IUndoableAction _undoableAction;

            public string Name => _undoableAction.Name;

            public DocumentUndoableAction(DataModelViewerViewModel document, IUndoableAction undoableAction)
            {
                _document = document;
                _selectedItems = _document.Viewer.LastSelection.Items.ToArray();
                _undoableAction = undoableAction;
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
        }
    }
}