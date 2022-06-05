using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Calame.Icons;
using Diese.Collections;
using Gemini.Framework;

namespace Calame.Utils
{
    public class AddCollectionItemCommand : ICommand
    {
        private readonly IList _list;
        private readonly IList<Type> _newTypeRegistry;
        private readonly IIconProvider _iconProvider;
        private readonly IIconDescriptor _iconDescriptor;

        private IList<Type> _newItemTypes;
        private readonly ICommand _addItemCommand;

        private bool CanAddItem => _list != null && !_list.IsFixedSize;
        public object AddedItem { get; private set; }

        public event EventHandler CanExecuteChanged;
        public event EventHandler ItemAdded;

        public AddCollectionItemCommand(IList list, IList<Type> newTypeRegistry, IIconProvider iconProvider, IIconDescriptor iconDescriptor)
        {
            _list = list;
            _newTypeRegistry = newTypeRegistry;
            _iconProvider = iconProvider;
            _iconDescriptor = iconDescriptor;
            _addItemCommand = new RelayCommand(x => AddItemOfType((Type)x));

            RefreshNewItemTypes();
        }

        public bool CanExecute(object _) => _newItemTypes != null && _newItemTypes.Count > 0;
        public void Execute(object _)
        {
            if (_newItemTypes.Count == 1)
            {
                AddItemOfType(_newItemTypes[0]);
                return;
            }

            var contextMenu = new ContextMenu();

            string[] typeNames = _newItemTypes.Select(x => x.Name).ToArray();
            ReduceTypeNamePatterns(typeNames);

            for (int i = 0; i < _newItemTypes.Count; i++)
            {
                var menuItem = new MenuItem
                {
                    Header = typeNames[i],
                    Command = _addItemCommand,
                    CommandParameter = _newItemTypes[i],
                    Icon = _iconProvider.GetControl(_iconDescriptor.GetTypeIcon(_newItemTypes[i]), 16)
                };

                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

        private void AddItemOfType(Type itemType) => AddItem(CreateItem(itemType));
        private void AddItem(object item)
        {
            _list.Add(item);

            AddedItem = item;
            ItemAdded?.Invoke(this, EventArgs.Empty);
        }

        private object CreateItem(Type type)
        {
            if (type.IsGenericType && type.GetConstructor(type.GenericTypeArguments) != null)
                return Activator.CreateInstance(type, type.GenericTypeArguments.Select(Activator.CreateInstance).ToArray());

            return Activator.CreateInstance(type);
        }

        private void RefreshNewItemTypes()
        {
            Type itemType = GetNewItemType();
            if (itemType == null)
                return;

            IList<Type> newItemTypes = _newTypeRegistry?.Where(x => itemType.IsAssignableFrom(x)).ToList() ?? new List<Type>();
            if (!newItemTypes.Contains(itemType) && IsInstantiableWithoutParameter(itemType))
                newItemTypes.Insert(0, itemType);

            _newItemTypes = newItemTypes;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private Type GetNewItemType()
        {
            if (!CanAddItem)
                return null;

            Type[] interfaces = _list.GetType().GetInterfaces();
            if (!interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>), out Type collectionType))
                return null;

            return collectionType.GenericTypeArguments[0];
        }

        static private bool IsInstantiableWithoutParameter(Type type)
        {
            if (type.IsValueType)
                return true;

            if (type.IsInterface)
                return false;
            if (type.IsAbstract)
                return false;
            if (type.IsGenericType && type.GetConstructor(type.GenericTypeArguments) != null)
                return false;
            if (type.GetConstructor(Type.EmptyTypes) == null)
                return false;

            return true;
        }

        static private void ReduceTypeNamePatterns(string[] values)
        {
            while (true)
            {
                int upperIndex = values[0].Skip(1).IndexOf(char.IsUpper) + 1;
                if (upperIndex <= 0)
                    break;

                string prefix = values[0].Substring(0, upperIndex);
                if (!values.Skip(1).All(x => x.StartsWith(prefix)))
                    break;

                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Substring(prefix.Length);
            }

            while (true)
            {
                int upperIndex = LastIndexOf(values[0], char.IsUpper);
                if (upperIndex == -1)
                    break;

                int suffixLength = values[0].Length - upperIndex;
                string suffix = values[0].Substring(upperIndex, suffixLength);
                if (!values.Skip(1).All(x => x.EndsWith(suffix)))
                    break;

                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Substring(0, values[i].Length - suffixLength);
            }
        }

        static public int LastIndexOf(string value, Predicate<char> predicate)
        {
            for (int i = value.Length - 1; i >= 0; i--)
                if (predicate(value[i]))
                    return i;
            return -1;
        }
    }
}