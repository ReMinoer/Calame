using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Fingear;

namespace Calame.InteractionTree.Converters
{
    public class InteractiveComponentsAndControlsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var interactive = (IInteractive)value;
            return interactive?.Components.Concat<object>(interactive.Controls) ?? Enumerable.Empty<object>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}