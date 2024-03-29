﻿using System;
using Gemini.Modules.UndoRedo;
using Glyph.Tools.UndoRedo;

namespace Calame
{
    public class UndoRedoUndoableAction : IUndoableAction, IDisposable
    {
        private readonly IUndoRedo _undoRedo;

        public UndoRedoUndoableAction(IUndoRedo undoRedo)
        {
            _undoRedo = undoRedo;
        }

        public string Name => _undoRedo.Description;
        public void Execute() => _undoRedo.Redo();
        public void Undo() => _undoRedo.Undo();
        public void Dispose() => _undoRedo.Dispose();
    }
}