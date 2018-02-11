using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Calame.DataModelTree.Converters
{
    public class ObjectToEnumerableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Enumerable(value) : null;

            IEnumerable Enumerable(object instance)
            {
                yield return instance;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : null;
        }
    }
}