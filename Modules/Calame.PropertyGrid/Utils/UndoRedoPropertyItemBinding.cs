using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Glyph.Tools.UndoRedo;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Calame.PropertyGrid.Utils
{
    public class UndoRedoPropertyItemBinding : Freezable, INotifyPropertyChanged
    {
        static public readonly DependencyProperty PropertyItemProperty =
            DependencyProperty.Register(nameof(PropertyItem), typeof(PropertyItem), typeof(UndoRedoPropertyItemBinding), new UIPropertyMetadata(null, OnPropertyItemChanged));

        public PropertyItem PropertyItem
        {
            get => (PropertyItem)GetValue(PropertyItemProperty);
            set => SetValue(PropertyItemProperty, value);
        }

        static public readonly DependencyProperty UndoRedoStackProperty =
            DependencyProperty.Register(nameof(UndoRedoStack), typeof(IUndoRedoStack), typeof(UndoRedoPropertyItemBinding), new UIPropertyMetadata(null));

        public IUndoRedoStack UndoRedoStack
        {
            get => (IUndoRedoStack)GetValue(UndoRedoStackProperty);
            set => SetValue(UndoRedoStackProperty, value);
        }

        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                object oldValue = Value;
                if (oldValue == value)
                    return;

                PropertyDescriptor propertyDescriptor = PropertyItem.PropertyDescriptor;
                object instance = PropertyItem.Instance;
                object newValue = value;

                if (newValue == oldValue)
                    return;

                UndoRedoStack.Execute($"Set property {propertyDescriptor.Name} of instance {instance} to {newValue}.",
                    () => propertyDescriptor.SetValue(instance, newValue),
                    () => propertyDescriptor.SetValue(instance, oldValue));
                
                NotifyPropertyChanged();
            }
        }

        static private void OnPropertyItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var undoRedoBinding = (UndoRedoPropertyItemBinding)d;

            var oldPropertyItem = (PropertyItem)e.OldValue;
            oldPropertyItem?.PropertyDescriptor.RemoveValueChanged(oldPropertyItem.Instance, undoRedoBinding.OnModelValueChanged);

            undoRedoBinding.RefreshValue();

            var newPropertyItem = (PropertyItem)e.NewValue;
            newPropertyItem?.PropertyDescriptor.AddValueChanged(newPropertyItem.Instance, undoRedoBinding.OnModelValueChanged);
        }

        private void OnModelValueChanged(object sender, EventArgs e)
        {
            if (CheckAccess())
            {
                RefreshValue();
            }
            else
            {
                Dispatcher.BeginInvoke((Action)RefreshValue);
            }
        }

        private void RefreshValue()
        {
            object newValue = PropertyItem?.PropertyDescriptor.GetValue(PropertyItem.Instance);
            if (newValue == _value)
                return;

            _value = newValue;
            NotifyPropertyChanged(nameof(Value));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected override Freezable CreateInstanceCore() => new UndoRedoPropertyItemBinding();
    }
}