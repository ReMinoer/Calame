using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Calame.PropertyGrid.ViewModels
{
    [Export(typeof(PropertyGridViewModel))]
    public sealed class PropertyGridViewModel : HandleTool, IHandle<ISelectionSpread<object>>
    {
        private object _selectedObject;
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public object SelectedObject
        {
            get => _selectedObject;
            set => SetValue(ref _selectedObject, value);
        }

        public IList<Type> NewItemTypeRegistry { get; }

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider)
            : base(eventAggregator)
        {
            DisplayName = "Property Grid";

            NewItemTypeRegistry = importedTypeProvider.Types.Where(t => t.GetConstructor(Type.EmptyTypes) != null).ToList();
        }

        public void Handle(ISelectionSpread<object> message) => SelectedObject = message.Item;
    }
}