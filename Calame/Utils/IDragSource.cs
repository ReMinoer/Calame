using System.Windows;

namespace Calame.Utils
{
    public interface IDragSource
    {
        DraggedData GetDraggedData();
    }

    public class DraggedData
    {
        public DragDropEffects Effect { get; set; }
        public DataObject Data { get; set; }

        public DraggedData(DragDropEffects effect, DataObject data)
        {
            Effect = effect;
            Data = data;
        }
    }
}