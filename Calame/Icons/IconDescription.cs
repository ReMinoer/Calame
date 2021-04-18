using System;
using System.Windows.Media;

namespace Calame.Icons
{
    public struct IconDescription : IEquatable<IconDescription>
    {
        static public IconDescription None => new IconDescription();

        public object Key { get; set; }
        public Brush Brush { get; set; }

        public bool Defined => Key != null;

        public IconDescription(object key, Brush brush)
        {
            Key = key;
            Brush = brush;
        }

        public bool Equals(IconDescription other) => Equals(Key, other.Key) && Equals(Brush, other.Brush);
        public override bool Equals(object obj) => obj is IconDescription other && Equals(other);
        public override int GetHashCode() => (Key != null ? Key.GetHashCode() : 0) ^ (Brush != null ? Brush.GetHashCode() : 0);
    }
}