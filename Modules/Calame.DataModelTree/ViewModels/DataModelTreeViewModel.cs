﻿using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework.Services;
using Glyph.Composition.Modelization;

namespace Calame.DataModelTree.ViewModels
{
    [Export(typeof(DataModelTreeViewModel))]
    public sealed class DataModelTreeViewModel : CalameTool<IDocumentContext<IGlyphData>>, IHandle<ISelectionSpread<IGlyphData>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }

        private IGlyphData _root;
        private IGlyphData _selection;
        private readonly TreeViewItemModelBuilder<IGlyphCreator> _treeItemBuilder;

        public IGlyphData Root
        {
            get => _root;
            private set => SetValue(ref _root, value);
        }

        public IGlyphData Selection
        {
            get => _selection;
            set
            {
                if (!SetValue(ref _selection, value))
                    return;

                var selectionRequest = new SelectionRequest<IGlyphData>(CurrentDocument, _selection);
                EventAggregator.PublishAsync(selectionRequest).Wait();
            }
        }

        protected override object IconKey => CalameIconKey.DataModelTree;

        [ImportingConstructor]
        public DataModelTreeViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Data Model Tree";

            IconProvider = iconProvider;
            IconDescriptor = iconDescriptorManager.GetDescriptor();

            IIconDescriptor<IGlyphData> iconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphData>();

            _treeItemBuilder = new TreeViewItemModelBuilder<IGlyphCreator>()
                               .DisplayName(x => x.DisplayName, nameof(IGlyphCreator.DisplayName))
                               .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Children), nameof(IGlyphCreator.Children))
                               .IconDescription(x => iconDescriptor.GetIcon(x));
        }
        
        protected override Task OnDocumentActivated(IDocumentContext<IGlyphData> activeDocument)
        {
            _selection = null;
            Root = activeDocument.Context;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            _selection = null;
            Root = null;

            return Task.CompletedTask;
        }
        
        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            SetValue(ref _selection, message.Item, nameof(Selection));
            return Task.CompletedTask;
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            return _treeItemBuilder.Build((IGlyphCreator)data, synchronizerConfiguration);
        }

        public bool DisableChildrenIfParentDisabled => false;
        event EventHandler ITreeContext.BaseFilterChanged { add { } remove { } }
        bool ITreeContext.IsMatchingBaseFilter(object data) => true;
    }
}