using System.Windows;

namespace Calame.Utils
{
    public interface IDropTarget
    {
        void OnDragEnter(DragEventArgs eventArgs);
        void OnDragOver(DragEventArgs eventArgs);
        void OnDragLeave(DragEventArgs eventArgs);
        void OnDrop(DragEventArgs eventArgs);
    }
}