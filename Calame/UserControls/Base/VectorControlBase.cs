using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.Primitives;

namespace Calame.UserControls.Base
{
    public abstract class VectorControlBase<TControl, TVector, TComponent> : UserControl
        where TControl : UpDownBase<TComponent>, new()
    {
        static public readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(TVector), typeof(VectorControlBase<TControl, TVector, TComponent>), new PropertyMetadata(default(TVector), PropertyChangedCallback));
        
        public TVector Value
        {
            get => (TVector)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private readonly TControl[] _controls;

        protected VectorControlBase(int componentCount)
        {
            var grid = new Grid();

            _controls = new TControl[componentCount];
            for (int i = 0; i < componentCount; i++)
            {
                var control = new TControl { TextAlignment = TextAlignment.Left };
                control.ValueChanged += ControlOnValueChanged;

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                Grid.SetColumn(control, i);
                grid.Children.Add(control);
                
                _controls[i] = control;
            }

            Content = grid;
        }
        
        static private void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var vectorControlBase = (VectorControlBase<TControl, TVector, TComponent>)dependencyObject;

            if (Equals(e.NewValue, null))
            {
                foreach (TControl control in vectorControlBase._controls)
                    control.Value = default(TComponent);
                return;
            }

            for (int i = 0; i < vectorControlBase._controls.Length; i++)
                vectorControlBase._controls[i].Value = vectorControlBase.GetComponent((TVector)e.NewValue, i);
        }

        private void ControlOnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            TVector value = Value;
            UpdateVector(ref value, _controls);
            Value = value;
        }
        
        protected abstract void UpdateVector(ref TVector vector, TControl[] controls);
        protected abstract TComponent GetComponent(TVector vector, int index);
    }
}