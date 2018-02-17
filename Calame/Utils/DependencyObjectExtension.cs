using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Calame.Utils
{
    static public class DependencyObjectExtension
    {
        static public IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                yield return VisualTreeHelper.GetChild(parent, i);
        }
    }
}