using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace Calame.Behaviors
{
    public class TextBlockHighlightFilteredBehavior : Behavior<TextBlock>
    {
        static public readonly DependencyProperty ItemTextProperty =
            DependencyProperty.Register(nameof(ItemText), typeof(string), typeof(TextBlockHighlightFilteredBehavior), new UIPropertyMetadata(null, UpdateHighlightFilter));

        public string ItemText
        {
            get => (string)GetValue(ItemTextProperty);
            set => SetValue(ItemTextProperty, value);
        }

        static public readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(TextBlockHighlightFilteredBehavior), new UIPropertyMetadata(null, UpdateHighlightFilter));

        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }

        static public readonly DependencyProperty HighlightForegroundProperty =
            DependencyProperty.Register(nameof(HighlightForeground), typeof(Brush), typeof(TextBlockHighlightFilteredBehavior), new UIPropertyMetadata(SystemColors.ControlTextBrush, UpdateHighlightFilter));

        public Brush HighlightForeground
        {
            get => (Brush)GetValue(HighlightForegroundProperty);
            set => SetValue(HighlightForegroundProperty, value);
        }

        static public readonly DependencyProperty HighlightBackgroundProperty =
            DependencyProperty.Register(nameof(HighlightBackground), typeof(Brush), typeof(TextBlockHighlightFilteredBehavior), new UIPropertyMetadata(null, UpdateHighlightFilter));

        public Brush HighlightBackground
        {
            get => (Brush)GetValue(HighlightBackgroundProperty);
            set => SetValue(HighlightBackgroundProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateHighlightFilter();
        }

        private void UpdateHighlightFilter() => UpdateHighlightFilter(this);

        static private void UpdateHighlightFilter(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateHighlightFilter((TextBlockHighlightFilteredBehavior)sender);
        }

        static private void UpdateHighlightFilter(TextBlockHighlightFilteredBehavior behavior)
        {
            TextBlock textBlock = behavior.AssociatedObject;
            if (textBlock == null)
                return;

            string text = behavior.ItemText;
            textBlock.Inlines.Clear();

            if (string.IsNullOrEmpty(text))
            {
                textBlock.Inlines.Add(new Run(text));
                return;
            }

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

                textBlock.Inlines.Add(new Run(text.Substring(occurrenceIndex, filterText.Length))
                {
                    Foreground = behavior.HighlightForeground,
                    Background = behavior.HighlightBackground
                });

                currentIndex = occurrenceIndex + filterText.Length;
            }

            if (currentIndex < text.Length)
                textBlock.Inlines.Add(new Run(text.Substring(currentIndex, text.Length - currentIndex)));
        }
    }
}