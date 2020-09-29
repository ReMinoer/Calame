using System;
using System.Globalization;
using System.Windows.Data;

namespace Calame.Converters
{
    public class PathToWatermarkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case null: return "None";
                case "": return parameter ?? "Root folder";
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}