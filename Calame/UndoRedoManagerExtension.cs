using System;
using Gemini.Modules.UndoRedo;
using Glyph.Tools.UndoRedo;

namespace Calame
{
    static public class UndoRedoManagerExtension
    {
        static public void Push(this IUndoRedoManager undoRedoManager, string description, Action redoAction, Action undoAction, Action doDispose = null, Action undoDispose = null)
        {
            undoRedoManager?.PushAction(new UndoRedoUndoableAction(new UndoRedoAction(description, redoAction, undoAction, doDispose, undoDispose)));
        }

        static public void Execute(this IUndoRedoManager undoRedoManager, string description, Action doAction, Action undoAction, Action doDispose = null, Action undoDispose = null)
        {
            doAction();
            undoRedoManager.Push(description, doAction, undoAction, doDispose, undoDispose);
        }
    }
}