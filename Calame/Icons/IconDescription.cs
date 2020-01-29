using System.Windows.Media;

namespace Calame.Icons
{
    public struct IconDescription
    {
        static public IconDescription None => new IconDescription();

        public object Key { get; }
        public Brush Brush { get; }

        public bool Defined => Key != null;

        public IconDescription(object key, Brush brush)
        {
            Key = key;
            Brush = brush;
        }
    }
}