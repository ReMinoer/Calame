using System.Windows.Controls;

namespace Calame.Icons
{
    public interface IIconProvider
    {
        Control GetControl(IconDescription iconDescription, int size);
    }
}