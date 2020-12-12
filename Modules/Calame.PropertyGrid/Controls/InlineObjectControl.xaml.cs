using System;
using System.Windows;
using Calame.Icons;

namespace Calame.PropertyGrid.Controls
{
    public partial class InlineObjectControl : PropertyGridPopupOwnerBase
    {
        static public readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(InlineObjectControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        static public readonly DependencyProperty NewItemTypeProperty =
            DependencyProperty.Register(nameof(NewItemType), typeof(Type), typeof(InlineObjectControl), new PropertyMetadata(null, OnNewItemTypeChanged));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public Type NewItemType
        {
            get => (Type)GetValue(NewItemTypeProperty);
            set => SetValue(NewItemTypeProperty, value);
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

        public override bool CanAddItem => !IsReadOnly && Value == GetDefaultItemValue();
        public override bool CanRemoveItem => !IsReadOnly && Value != GetDefaultItemValue();

        static private void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineObjectControl)d;

            if (control.NewItemType == null)
                control.RefreshNewItemTypes();

            control.RefreshAccessIcon();
            control.OnPropertyChanged(nameof(CanAddItem));
            control.OnPropertyChanged(nameof(CanRemoveItem));
        }

        static private void OnNewItemTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineObjectControl)d;
            control.RefreshNewItemTypes();
        }

        protected override void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            RefreshAccessIcon();
            OnPropertyChanged(nameof(CanAddItem));
            OnPropertyChanged(nameof(CanRemoveItem));
        }

        private void RefreshAccessIcon()
        {
            AccessIconKey = IsPropertyGridReadOnly ? CalameIconKey.ReadOnlyProperties : CalameIconKey.EditableProperties;
        }

        public InlineObjectControl()
        {
            InitializeComponent();
        }

        protected override Type GetNewItemType()
        {
            return NewItemType ?? Value?.GetType();
        }

        private object GetDefaultItemValue()
        {
            Type itemType = GetNewItemType();
            if (itemType == null)
                return null;

            return itemType.IsValueType ? Activator.CreateInstance(itemType) : null;
        }

        protected override void OnAddItem(object type)
        {
            Value = Activator.CreateInstance((Type)type);

            OnExpandObject(ObjectButton);
            OnPropertyChanged(nameof(CanAddItem));
            OnPropertyChanged(nameof(CanRemoveItem));
        }

        protected override void OnItemRemoved(DependencyObject popupOwner)
        {
            Value = GetDefaultItemValue();

            OnPropertyChanged(nameof(CanAddItem));
            OnPropertyChanged(nameof(CanRemoveItem));
        }

        protected override void RefreshValueType(DependencyObject popupOwner, object value)
        {
            Value = value;
        }
    }
}
