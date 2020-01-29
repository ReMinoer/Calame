using System;
using System.Globalization;
using System.Windows.Data;
using Calame.Icons;

namespace Calame.Converters
{
    public class ModelToIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object model = values[0];
            var iconDescriptor = (IIconDescriptor)values[1];
            var iconTargetSelector = (IIconTargetSelector)values[2];

            return iconDescriptor?.GetIcon(iconTargetSelector?.GetIconTarget(model) ?? model) ?? IconDescription.None;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new []{ Binding.DoNothing, Binding.DoNothing };
        }
    }
}