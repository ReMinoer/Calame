using System;
using System.Globalization;
using System.Windows.Data;

namespace Calame.PropertyGrid.Converters
{
    public class MonoGameToWpfColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            
            var monogameColor = (Microsoft.Xna.Framework.Color)value;
            return System.Windows.Media.Color.FromArgb(monogameColor.A, monogameColor.R, monogameColor.G, monogameColor.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var wpfColor = (System.Windows.Media.Color)value;
            return new Microsoft.Xna.Framework.Color(wpfColor.R, wpfColor.G, wpfColor.B, wpfColor.A);
        }
    }
}