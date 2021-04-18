using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Xaml.Behaviors;

namespace Calame.Behaviors
{
    public class TextBlockHighlightFilteredBehavior : Behavior<TextBlock>
    {
        static public readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(TextBlockHighlightFilteredBehavior), new UIPropertyMetadata(null, OnFilterTextChanged));

        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }

        static private void OnFilterTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (TextBlockHighlightFilteredBehavior)sender;
            TextBlock textBlock = behavior.AssociatedObject;
            if (textBlock == null)
                return;

            string text = textBlock.Text;

            textBlock.Inlines.Clear();
            if (string.IsNullOrEmpty(text))
                return;
            
            string filterText = behavior.FilterText;

            if (string.IsNullOrEmpty(filterText))
            {
                textBlock.Inlines.Add(new Run(text));
                return;
            }

            int currentIndex = 0;

            while (true)
            {
                int occurrenceIndex = text.IndexOf(filterText, currentIndex, StringComparison.OrdinalIgnoreCase);
                if (occurrenceIndex == -1)
                    break;

                if (currentIndex < occurrenceIndex)
                    textBlock.Inlines.Add(new Run(text.Substring(currentIndex, occurrenceIndex - currentIndex)));

                textBlock.Inlines.Add(new Bold(new Run(text.Substring(occurrenceIndex, filterText.Length))));

                currentIndex = occurrenceIndex + filterText.Length;
            }

            if (currentIndex < text.Length)
                textBlock.Inlines.Add(new Run(text.Substring(currentIndex, text.Length - currentIndex)));
        }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}