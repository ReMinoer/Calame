using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Calame.DocumentContexts;
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
    public sealed class DataModelTreeViewModel : CalameTool<IDocumentContext<IRootDataContext>>, IHandle<ISelectionSpread<IGlyphData>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }

        private ISelectionContext<IGlyphData> _selectionContext;
        private readonly TreeViewItemModelBuilder<IGlyphData> _dataItemBuilder;
        private readonly TreeViewItemModelBuilder<IReadOnlyObservableCollection<IGlyphData>> _childrenSourceItemBuilder;

        private IRootDataContext _rootDataContext;
        public IRootDataContext RootDataContext
        {
            get => _rootDataContext;
            private set => SetValue(ref _rootDataContext, value);
        }

        private object _selection;
        public object Selection
        {
            get => _selection;
            set
            {
                if (!SetValue(ref _selection, value))
                    return;

                if (_selection is IGlyphData data)
                    _selectionContext.SelectAsync(data).Wait();
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

            IIconDescriptor defaultIconDescriptor = iconDescriptorManager.GetDescriptor();
            IIconDescriptor<IGlyphData> dataIconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphData>();

            _dataItemBuilder = new TreeViewItemModelBuilder<IGlyphData>()
                .DisplayName(x => x.DisplayName, nameof(IGlyphData.DisplayName))
                .ChildrenSource(x => new CompositeReadOnlyObservableList<object>
                (
                    new EnumerableReadOnlyObservableList<object>(x.Children),
                    new EnumerableReadOnlyObservableList<object>(x.ChildrenSources)
                ), x => ObservableHelpers.OnPropertyChanged(x as INotifyPropertyChanged, nameof(IGlyphData.Children), nameof(IGlyphData.ChildrenSources)))
                .IconDescription(x => dataIconDescriptor.GetIcon(x));

            _childrenSourceItemBuilder = new TreeViewItemModelBuilder<IReadOnlyObservableCollection<IGlyphData>>()
                .DisplayName(x => x.ToString())
                .FontWeight(_ => FontWeights.Bold)
                .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x))
                .IconDescription(x => defaultIconDescriptor.GetIcon(x))
                .IsHeader(_ => true);
        }
        
        protected override Task OnDocumentActivated(IDocumentContext<IRootDataContext> activeDocument)
        {
            _selection = null;

            _selectionContext = activeDocument.GetSelectionContext<IGlyphData>();
            RootDataContext = activeDocument.Context;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            _selection = null;

            RootDataContext = null;
            _selectionContext = null;

            return Task.CompletedTask;
        }
        
        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            SetValue(ref _selection, message.Item, nameof(Selection));
            return Task.CompletedTask;
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            switch (data)
            {
                case IGlyphData glyphData:
                    return _dataItemBuilder.Build(glyphData, synchronizerConfiguration);
                case IReadOnlyObservableCollection<IGlyphData> childrenSource:
                    return _childrenSourceItemBuilder.Build(childrenSource, synchronizerConfiguration);
                default:
                    throw new NotSupportedException();
            }
        }

        public bool DisableChildrenIfParentDisabled => false;
        event EventHandler ITreeContext.BaseFilterChanged { add { } remove { } }
        bool ITreeContext.IsMatchingBaseFilter(object data) => true;
    }
}