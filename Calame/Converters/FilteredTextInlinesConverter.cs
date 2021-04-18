using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;

namespace Calame.Converters
{
    public class FilteredTextInlinesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string text = values[0]?.ToString();
            string filterText = values[1]?.ToString();

            if (string.IsNullOrEmpty(filterText))
                return new Run(text);

            var inlines = new List<Inline>();
            int currentIndex = 0;

            while (true)
            {
                int occurrenceIndex = text.IndexOf(filterText, currentIndex, StringComparison.OrdinalIgnoreCase);
                if (occurrenceIndex == -1)
                    break;

                if (currentIndex < occurrenceIndex)
                    inlines.Add(new Run(text.Substring(currentIndex, occurrenceIndex - currentIndex)));

                inlines.Add(new Bold(new Run(text.Substring(occurrenceIndex, filterText.Length))));

                currentIndex = occurrenceIndex + filterText.Length;
            }

            if (currentIndex < text.Length)
                inlines.Add(new Run(text.Substring(currentIndex, text.Length - currentIndex)));

            return inlines;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new []{ Binding.DoNothing, Binding.DoNothing};
        }
    }
}