using System;
using System.Drawing;
using System.Windows.Controls;

namespace Calame.Icons
{
    public interface IIconProvider
    {
        Control GetControl(IconDescription iconDescription, int size);
        Bitmap GetBitmap(IconDescription iconDescription, int size);
        Uri GetUri(IconDescription iconDescription, int size);
    }
}