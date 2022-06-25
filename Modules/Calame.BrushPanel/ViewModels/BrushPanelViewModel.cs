using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Calame.Viewer.Messages;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.BrushPanel.ViewModels
{
    [Export(typeof(BrushPanelViewModel))]
    public sealed class BrushPanelViewModel : CalameTool<IDocumentContext<ViewerViewModel>>, ITreeContext, IHandle<ISelectionSpread<IGlyphComponent>>, IHandle<ISelectionSpread<IGlyphData>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public IIconProvider IconProvider { get; }
        public IIconDescriptorManager IconDescriptorManager { get; }
        public IIconDescriptor IconDescriptor { get; }

        private IBrushViewerModule _viewerModule;
        private ISelectionContext _selectionContext;

        private readonly TreeViewItemModelBuilder<IGlyphData> _dataTreeItemBuilder;
        private readonly TreeViewItemModelBuilder<IGlyphDataChildrenSource> _childrenSourceItemBuilder;
        private readonly TreeViewItemModelBuilder<IGlyphComponent> _componentTreeItemBuilder;

        public bool DisableChildrenIfParentDisabled => true;
        public event EventHandler BaseFilterChanged;

        private readonly IEngineBrushViewModel[] _allEngineBrushes;
        private readonly IDataBrushViewModel[] _allDataBrushes;
        private readonly ObservableCollection<IBrushViewModel> _brushes;
        public IReadOnlyObservableCollection<IBrushViewModel> Brushes { get; }

        private IRootsContext _rootsContext;
        public IRootsContext RootsContext
        {
            get => _rootsContext;
            private set => SetValue(ref _rootsContext, value);
        }

        private object _selectedCanvas;
        public object SelectedCanvas
        {
            get => _selectedCanvas;
            set
            {
                if (!SetValue(ref _selectedCanvas, value))
                    return;

                OnCanvasChanged();
                _selectionContext?.SelectAsync(_selectedCanvas).Wait();

                if (_selectedCanvas != null)
                    SwitchToBrushModeAsync().Wait();
            }
        }

        private IBrushViewModel _selectedBrush;
        public IBrushViewModel SelectedBrush
        {
            get => _selectedBrush;
            set
            {
                if (SetValue(ref _selectedBrush, value))
                    OnBrushChanged();
            }
        }

        private IPaintViewModel _selectedPaint;
        public IPaintViewModel SelectedPaint
        {
            get => _selectedPaint;
            set
            {
                if (SetValue(ref _selectedPaint, value))
                    OnPaintChanged();
            }
        }

        public ICommand SelectBrushCommand { get; }
        public ICommand SelectPaintCommand { get; }

        protected override object IconKey => CalameIconKey.BrushPanel;

        [ImportingConstructor]
        public BrushPanelViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEngineBrushViewModel[] allEngineBrushes, [ImportMany] IDataBrushViewModel[] allDataBrushes)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Brush Panel";

            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;
            IconDescriptor = iconDescriptorManager.GetDescriptor();

            IIconDescriptor defaultIconDescriptor = iconDescriptorManager.GetDescriptor();
            IIconDescriptor<IGlyphData> dataIconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphData>();
            IIconDescriptor<IGlyphComponent> componentIconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();

            _dataTreeItemBuilder = new TreeViewItemModelBuilder<IGlyphData>()
                .DisplayName(x => x.DisplayName, nameof(IGlyphData.DisplayName))
                .ChildrenSource(x => new CompositeReadOnlyObservableList<object>
                (
                    new EnumerableReadOnlyObservableList<object>(x.Children),
                    new EnumerableReadOnlyObservableList<object>(x.ChildrenSources)
                ), x => ObservableHelpers.OnPropertyChanged(x as INotifyPropertyChanged, nameof(IGlyphData.Children), nameof(IGlyphData.ChildrenSources)))
                .IconDescription(dataIconDescriptor.GetIcon);

            _childrenSourceItemBuilder = new TreeViewItemModelBuilder<IGlyphDataChildrenSource>()
                .DisplayName(x => x.PropertyName)
                .FontWeight(_ => FontWeights.Bold)
                .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Children), nameof(IGlyphDataChildrenSource.Children))
                .IconDescription(x => IconDescriptor.GetIcon(x.Children), nameof(IGlyphDataChildrenSource.Children))
                .IsHeader(_ => true);

            _componentTreeItemBuilder = new TreeViewItemModelBuilder<IGlyphComponent>()
                .DisplayName(x => x.Name, nameof(IGlyphComponent.Name))
                .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Components), nameof(IGlyphComponent.Components))
                .IconDescription(componentIconDescriptor.GetIcon);

            SelectBrushCommand = new RelayCommand(OnSelectBrush);
            SelectPaintCommand = new RelayCommand(OnSelectPaint);

            _allEngineBrushes = allEngineBrushes;
            _allDataBrushes = allDataBrushes;
            _brushes = new ObservableCollection<IBrushViewModel>();
            Brushes = new ReadOnlyObservableCollection<IBrushViewModel>(_brushes);
        }

        protected override Task OnDocumentActivated(IDocumentContext<ViewerViewModel> activeDocument)
        {
            ViewerViewModel viewer = activeDocument.Context;

            _selectionContext = activeDocument.TryGetContext<ISelectionContext>();
            _viewerModule = viewer.Modules.FirstOfTypeOrDefault<IBrushViewerModule>();
            RootsContext = activeDocument.GetContext<IRootsContext>();

            //IBrush previousBrush = _viewerModule.Brush;
            //IPaint previousPaint = _viewerModule.Paint;

            HandleSelection(_viewerModule.Canvas);
            SelectedBrush = _brushes.FirstOrDefault();

            //TODO: Handle selection binding
            //SelectedBrush = _brushes.FirstOrDefault(x => x == previousBrush);
            //SelectedPaint = SelectedBrush?.Paints.FirstOrDefault(x => x.Paint == previousPaint);

            if (_selectionContext != null)
                _selectionContext.CanSelectChanged += OnCanSelectChanged;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            if (_selectionContext != null)
                _selectionContext.CanSelectChanged -= OnCanSelectChanged;

            HandleSelection(null);

            RootsContext = null;
            _selectionContext = null;
            _viewerModule = null;

            return Task.CompletedTask;
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object model, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            switch (model)
            {
                case IGlyphData data:
                    return _dataTreeItemBuilder.Build(data, synchronizerConfiguration);
                case IGlyphDataChildrenSource childrenSource:
                    return _childrenSourceItemBuilder.Build(childrenSource, synchronizerConfiguration);
                case IGlyphComponent component:
                    return _componentTreeItemBuilder.Build(component, synchronizerConfiguration);
                default:
                    throw new NotSupportedException();
            }
        }

        bool ITreeContext.IsMatchingBaseFilter(object model)
        {
            return GetAllBrushesForType(model).Any(brush => brush.IsValidForCanvas(model))
                && (_selectionContext?.CanSelect(model) ?? true);
        }

        private void OnCanSelectChanged(object sender, EventArgs e)
        {
            BaseFilterChanged?.Invoke(this, EventArgs.Empty);
        }

        private IEnumerable<IBrushViewModel> GetAllBrushesForType(object canvas)
        {
            switch (canvas)
            {
                case IGlyphData _:
                    return _allDataBrushes;
                case IGlyphComponent _:
                    return _allEngineBrushes;
                default:
                    return Enumerable.Empty<IBrushViewModel>();
            }
        }

        Task IHandle<ISelectionSpread<IGlyphComponent>>.HandleAsync(ISelectionSpread<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext != CurrentDocument)
                return Task.CompletedTask;

            HandleSelection(message.Item);
            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext != CurrentDocument)
                return Task.CompletedTask;

            HandleSelection(message.Item);
            return Task.CompletedTask;
        }

        private void HandleSelection(object canvas)
        {
            if (SetValue(ref _selectedCanvas, canvas, nameof(SelectedCanvas)))
                OnCanvasChanged();
        }

        private void OnCanvasChanged()
        {
            SelectedBrush = null;
            _brushes.Clear();

            if (SelectedCanvas != null)
                foreach (IBrushViewModel brush in GetAllBrushesForType(SelectedCanvas).Where(x => x.IsValidForCanvas(SelectedCanvas)))
                    _brushes.Add(brush);
            
            _viewerModule.Canvas = SelectedCanvas;
            SelectedBrush = _brushes.FirstOrDefault();
        }

        private void OnSelectBrush(object obj)
        {
            SelectedBrush = (IBrushViewModel)obj;
            SwitchToBrushModeAsync().Wait();
        }

        private void OnBrushChanged()
        {
            SelectedPaint = null;

            _viewerModule.Brush = SelectedBrush;
            SelectedPaint = SelectedBrush?.Paints.FirstOrDefault();
        }

        private void OnSelectPaint(object obj)
        {
            SelectedPaint = (IPaintViewModel)obj;
            SwitchToBrushModeAsync().Wait();
        }

        private void OnPaintChanged()
        {
            _viewerModule.Paint = SelectedPaint?.Paint;
        }

        private Task SwitchToBrushModeAsync()
        {
            return EventAggregator.PublishAsync(new SwitchViewerModeRequest<IBrushViewerModule>(CurrentDocument));
        }
    }
}