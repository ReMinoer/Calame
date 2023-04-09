using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calame.UserControls
{
    /// <summary>
    /// Interaction logic for EditableControl.xaml
    /// </summary>
    public partial class EditableTextControl : UserControl
    {
        static public readonly DependencyProperty CanEditProperty =
            DependencyProperty.Register(nameof(CanEdit), typeof(bool), typeof(EditableTextControl), new UIPropertyMetadata(true));
        static private readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register(nameof(EditMode), typeof(bool), typeof(EditableTextControl), new UIPropertyMetadata(false));
        static public readonly DependencyProperty DisplayContentProperty =
            DependencyProperty.Register(nameof(DisplayContent), typeof(object), typeof(EditableTextControl), new UIPropertyMetadata(null));
        static public readonly DependencyProperty DisplayTemplateProperty =
            DependencyProperty.Register(nameof(DisplayTemplate), typeof(DataTemplate), typeof(EditableTextControl), new UIPropertyMetadata(null));
        static public readonly DependencyProperty DisplayTemplateSelectorProperty =
            DependencyProperty.Register(nameof(DisplayTemplateSelector), typeof(DataTemplateSelector), typeof(EditableTextControl), new UIPropertyMetadata(null));
        static public readonly DependencyProperty EditableTextProperty =
            DependencyProperty.Register(nameof(EditableText), typeof(string), typeof(EditableTextControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static private string _initialText;

        public bool CanEdit
        {
            get => (bool)GetValue(CanEditProperty);
            set => SetValue(CanEditProperty, value);
        }

        public bool EditMode
        {
            get => (bool)GetValue(EditModeProperty);
            set => SetValue(EditModeProperty, value);
        }

        public object DisplayContent
        {
            get => GetValue(DisplayContentProperty);
            set => SetValue(DisplayContentProperty, value);
        }

        public DataTemplate DisplayTemplate
        {
            get => (DataTemplate)GetValue(DisplayTemplateProperty);
            set => SetValue(DisplayTemplateProperty, value);
        }

        public DataTemplateSelector DisplayTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DisplayTemplateSelectorProperty);
            set => SetValue(DisplayTemplateSelectorProperty, value);
        }

        public string EditableText
        {
            get => (string)GetValue(EditableTextProperty);
            set => SetValue(EditableTextProperty, value);
        }
        
        public EditableTextControl()
        {
            InitializeComponent();
            TextBox.IsVisibleChanged += OnTextBoxIsVisibleChanged;
        }

        private void OnTextBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                _initialText = TextBox.Text;
                TextBox.Focus();
                TextBox.SelectAll();
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!CanEdit)
                return;

            if (e.ClickCount == 2)
            {
                EditMode = true;
                e.Handled = true;
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            EditMode = false;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EditMode = false;
                e.Handled = true;
            }

            if (e.Key == Key.Escape)
            {
                TextBox.Text = _initialText;
                EditMode = false;
                e.Handled = true;
            }
        }
    }
}
