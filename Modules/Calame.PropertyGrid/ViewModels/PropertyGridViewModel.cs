﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;
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

        public ICommand DirtyDocumentCommand { get; }

        [ImportingConstructor]
        public PropertyGridViewModel(IShell shell, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator)
        {
            DisplayName = "Property Grid";

            NewItemTypeRegistry = importedTypeProvider.Types.Where(t => t.GetConstructor(Type.EmptyTypes) != null).ToList();
            
            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;

            DirtyDocumentCommand = new RelayCommand(x => EventAggregator.PublishAsync(new DirtyMessage(CurrentDocument, SelectedObject)).Wait());
        }

        protected override Task OnDocumentActivated(IDocumentContext activeDocument)
        {
            SelectedObject = null;
            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            SelectedObject = null;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ISelectionSpread<object> message, CancellationToken cancellationToken)
        {
            SelectedObject = message.Item;
            return Task.CompletedTask;
        }
    }
}