using System.Windows.Media;

namespace Calame.Icons
{
    public struct IconDescription
    {
        static public IconDescription None => new IconDescription();

        public object Key { get; }
        public Brush Brush { get; }
        public double Margin { get; }

        public bool Defined => Key != null;

        public IconDescription(object key, Brush brush, double margin = 0)
        {
            Key = key;
            Brush = brush;
            Margin = margin;
        }

        public override int GetHashCode()
        {
            return (Key?.GetHashCode() ?? 0) ^ (Brush?.ToString().GetHashCode() ?? 0) ^ Margin.GetHashCode();
        }
    }
}