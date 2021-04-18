using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calame.Icons;
using Gemini.Framework;

namespace Calame.UserControls
{
    /// <summary>
    /// Interaction logic for FilterTextBox.xaml
    /// </summary>
    public partial class FilterTextBox : UserControl
    {
        static public readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(FilterTextBox), new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        static public readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(IIconProvider), typeof(FilterTextBox), new PropertyMetadata());

        public IIconProvider IconProvider
        {
            get => (IIconProvider)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        static public readonly DependencyProperty IconDescriptorProperty =
            DependencyProperty.Register(nameof(IconDescriptor), typeof(IIconDescriptor), typeof(FilterTextBox), new PropertyMetadata());

        public IIconDescriptor IconDescriptor
        {
            get => (IIconDescriptor)GetValue(IconDescriptorProperty);
            set => SetValue(IconDescriptorProperty, value);
        }

        public ICommand ClearFilterCommand { get; }

        public FilterTextBox()
        {
            ClearFilterCommand = new RelayCommand(OnClearFilter);
            InitializeComponent();
        }

        private void OnClearFilter(object _)
        {
            Text = string.Empty;
        }
    }
}
