using System.Windows.Media;

namespace Calame.Icons
{
    public struct IconDescription
    {
        static public IconDescription None => new IconDescription();

        public object Key { get; set; }
        public Brush Brush { get; set; }
        public double Margin { get; set; }

        public bool Defined => Key != null;

        public IconDescription(object key, Brush brush, double margin = 0)
        {
            Key = key;
            Brush = brush;
            Margin = margin;
        }
    }
}