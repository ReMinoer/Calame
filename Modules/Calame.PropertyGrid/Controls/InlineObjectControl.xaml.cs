using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Calame.Icons;

namespace Calame.PropertyGrid.Controls
{
    public partial class InlineObjectControl : PropertyGridPopupOwnerBase
    {
        static public readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(InlineObjectControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueOrReadOnlyChanged));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private object _accessIconKey = CalameIconKey.EditableProperties;
        public object AccessIconKey
        {
            get => _accessIconKey;
            private set
            {
                if (_accessIconKey == value)
                    return;

                _accessIconKey = value;
                OnPropertyChanged();
            }
        }

        protected override Type PropertyGridDisplayedType => Value?.GetType();
        public override bool IsItemsSourceResizable => false;

        static InlineObjectControl()
        {
            IsReadOnlyProperty.OverrideMetadata(typeof(InlineObjectControl), new PropertyMetadata(false, OnValueOrReadOnlyChanged));
        }

        static private void OnValueOrReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineObjectControl)d;
            control.AccessIconKey = control.IsPropertyGridReadOnly ? CalameIconKey.ReadOnlyProperties : CalameIconKey.EditableProperties;
        }

        public InlineObjectControl()
        {
            InitializeComponent();
        }

        protected override void OnItemRemoved(DependencyObject popupOwner)
        {
        }

        protected override void RefreshValueType(DependencyObject popupOwner, object value)
        {
            Value = value;
        }
    }
}
