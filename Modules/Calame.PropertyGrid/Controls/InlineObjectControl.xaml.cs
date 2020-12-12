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

        static public readonly DependencyProperty BaseTypeProperty =
            DependencyProperty.Register(nameof(BaseType), typeof(Type), typeof(InlineObjectControl), new PropertyMetadata(null, OnBaseTypeChanged));
        static public readonly DependencyProperty NewItemBaseTypeProperty =
            DependencyProperty.Register(nameof(NewItemBaseType), typeof(Type), typeof(InlineObjectControl), new PropertyMetadata(null, OnNewItemBaseTypeChanged));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public Type BaseType
        {
            get => (Type)GetValue(BaseTypeProperty);
            set => SetValue(BaseTypeProperty, value);
        }

        public Type NewItemBaseType
        {
            get => (Type)GetValue(NewItemBaseTypeProperty);
            set => SetValue(NewItemBaseTypeProperty, value);
        }

        private object _accessIconKey = CalameIconKey.EditableItem;
        public object AccessIconKey
        {
            get => _accessIconKey;
            private set => Set(ref _accessIconKey, value);
        }

        private bool _canAddItem;
        public override bool CanAddItem => _canAddItem;
        private void SetCanAddItem(bool value) => Set(ref _canAddItem, value, nameof(CanAddItem));

        private bool _canRemoveItem;
        public override bool CanRemoveItem => _canRemoveItem;
        private void SetCanRemoveItem(bool value) => Set(ref _canRemoveItem, value, nameof(CanRemoveItem));

        static InlineObjectControl()
        {
            IsReadOnlyValueProperty.OverrideMetadata(typeof(InlineObjectControl), new PropertyMetadata(false, OnIsReadOnlyValueChanged));
        }

        public InlineObjectControl()
        {
            InitializeComponent();
        }

        static private void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineObjectControl)d;

            if (control.BaseType == null && control.NewItemBaseType == null)
                control.RefreshNewItemTypes();

            control.RefreshCanAddItem();
            control.RefreshCanRemoveItem();
            control.RefreshAccessIcon();
        }

        static private void OnBaseTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineObjectControl)d;

            if (control.NewItemBaseType == null)
                control.RefreshNewItemTypes();

            control.RefreshCanAddItem();
            control.RefreshCanRemoveItem();
            control.RefreshAccessIcon();
        }

        static private void OnNewItemBaseTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineObjectControl)d;

            control.RefreshNewItemTypes();
            control.RefreshCanAddItem();
            control.RefreshCanRemoveItem();
            control.RefreshAccessIcon();
        }

        protected override void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsReadOnlyChanged(e);

            RefreshCanAddItem();
            RefreshCanRemoveItem();
            RefreshAccessIcon();
        }

        static private void OnIsReadOnlyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (InlineObjectControl)d;

            control.RefreshCanAddItem();
            control.RefreshCanRemoveItem();
            control.RefreshAccessIcon();
        }

        protected override Type GetNewItemType() => NewItemBaseType ?? GetItemType();
        private Type GetItemType() => BaseType ?? Value?.GetType();

        protected override void OnAddItem(object type)
        {
            Value = Activator.CreateInstance((Type)type);
            OnExpandObject(ObjectButton);
        }

        protected override void OnItemRemoved(DependencyObject popupOwner)
        {
            Type itemType = GetNewItemType();
            Value = itemType.IsValueType ? Activator.CreateInstance(itemType) : null;
        }

        protected override void RefreshValueType(DependencyObject popupOwner, object value)
        {
            Value = value;
        }

        private void RefreshCanAddItem()
        {
            bool value = !IsReadOnlyValue && Value == null;
            SetCanAddItem(value);
        }

        private void RefreshCanRemoveItem()
        {
            bool value = !IsReadOnlyValue && (!GetItemType()?.IsValueType ?? false) && Value != null;
            SetCanRemoveItem(value);
        }

        private void RefreshAccessIcon()
        {
            if (IsPropertyGridReadOnly)
                AccessIconKey = CalameIconKey.ReadOnlyProperties;
            else if (IsReadOnlyValue)
                AccessIconKey = CalameIconKey.EditableProperties;
            else
                AccessIconKey = CalameIconKey.EditableItem;
        }
    }
}
