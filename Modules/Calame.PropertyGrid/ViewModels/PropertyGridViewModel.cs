using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Calame.PropertyGrid.ViewModels
{
    [Export(typeof(PropertyGridViewModel))]
    public sealed class PropertyGridViewModel : CalameTool<IDocumentContext>, IHandle<ISelectionSpread<object>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;
        
        public IIconProvider IconProvider { get; }
        public IIconDescriptorManager IconDescriptorManager { get; }

        private object _selectedObject;
        public object SelectedObject
        {
            get => _selectedObject;
            set => SetValue(ref _selectedObject, value);
        }

        public IList<Type> NewItemTypeRegistry { get; }

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator)
        {
            DisplayName = "Property Grid";

            NewItemTypeRegistry = importedTypeProvider.Types.Where(t => t.GetConstructor(Type.EmptyTypes) != null).ToList();
            
            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;
        }

        protected override void OnDocumentActivated(IDocumentContext activeDocument)
        {
            SelectedObject = null;
        }

        protected override void OnDocumentsCleaned()
        {
            SelectedObject = null;
        }

        public void Handle(ISelectionSpread<object> message) => SelectedObject = message.Item;
    }
}