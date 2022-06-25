using Glyph.Tools.UndoRedo;

namespace Calame.DocumentContexts
{
    public interface IUndoRedoContext
    {
        IUndoRedoStack UndoRedoStack { get; }
    }
}