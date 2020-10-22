using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Calame.Viewer;
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
using Glyph.Engine;

namespace Calame.BrushPanel.ViewModels
{
    [Export(typeof(BrushPanelViewModel))]
    public sealed class BrushPanelViewModel : CalameTool<IDocumentContext<ViewerViewModel>>, ITreeContext, IHandle<ISelectionSpread<IGlyphComponent>>, IHandle<ISelectionSpread<IGlyphData>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public IIconProvider IconProvider { get; }
        public IIconDescriptorManager IconDescriptorManager { get; }

        private readonly TreeViewItemModelBuilder<IGlyphData> _dataTreeItemBuilder;
        private readonly TreeViewItemModelBuilder<IGlyphComponent> _componentTreeItemBuilder;
        
        private GlyphEngine _engine;
        private IBrushViewerModule _viewerModule;
        private IDocumentContext<IComponentFilter> _filteringContext;

        private readonly IEngineBrushViewModel[] _allEngineBrushes;
        private readonly IDataBrushViewModel[] _allDataBrushes;
        private readonly ObservableCollection<IBrushViewModel> _brushes;
        public IReadOnlyObservableCollection<IBrushViewModel> Brushes { get; }

        private IEnumerable _items;
        public IEnumerable Items
        {
            get => _items;
            private set => SetValue(ref _items, value);
        }

        private object _selectedCanvas;
        public object SelectedCanvas
        {
            get => _selectedCanvas;
            set
            {
                object previousCanvas = _selectedCanvas;

                if (!SetValue(ref _selectedCanvas, value))
                    return;

                OnCanvasChanged();

                ISelectionRequest<object> selectionRequest;
                if (_selectedCanvas != null)
                {
                    switch (_selectedCanvas)
                    {
                        case IGlyphData data:
                            selectionRequest = new SelectionRequest<IGlyphData>(CurrentDocument, data);
                            break;
                        case IGlyphComponent component:
                            selectionRequest = new SelectionRequest<IGlyphComponent>(CurrentDocument, component);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
                else
                {
                    switch (previousCanvas)
                    {
                        case IGlyphData _:
                            selectionRequest = new SelectionRequest<IGlyphData>(CurrentDocument, (IGlyphData)null);
                            break;
                        case IGlyphComponent _:
                            selectionRequest = new SelectionRequest<IGlyphComponent>(CurrentDocument, (IGlyphComponent)null);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                EventAggregator.PublishAsync(selectionRequest).Wait();
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

        [ImportingConstructor]
        public BrushPanelViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEngineBrushViewModel[] allEngineBrushes, [ImportMany] IDataBrushViewModel[] allDataBrushes)
            : base(shell, eventAggregator)
        {
            DisplayName = "Brush Panel";

            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;

            IIconDescriptor<IGlyphComponent> iconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();

            _dataTreeItemBuilder = new TreeViewItemModelBuilder<IGlyphData>()
                                   .DisplayName(x => x.DisplayName, nameof(IGlyphData.DisplayName))
                                   .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Children), nameof(IGlyphData.Children))
                                   .IconDescription(x => iconDescriptor.GetIcon(x.BindedObject));

            _componentTreeItemBuilder = new TreeViewItemModelBuilder<IGlyphComponent>()
                                        .DisplayName(x => x.Name, nameof(IGlyphComponent.Name))
                                        .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Components), nameof(IGlyphComponent.Components))
                                        .IconDescription(x => iconDescriptor.GetIcon(x));

            SelectBrushCommand = new RelayCommand(OnSelectBrush);
            SelectPaintCommand = new RelayCommand(OnSelectPaint);

            _allEngineBrushes = allEngineBrushes;
            _allDataBrushes = allDataBrushes;
            _brushes = new ObservableCollection<IBrushViewModel>();
            Brushes = new ReadOnlyObservableCollection<IBrushViewModel>(_brushes);

            if (shell.ActiveItem is IDocumentContext<ViewerViewModel> documentContext)
                _viewerModule = documentContext.Context.Modules.FirstOfTypeOrDefault<IBrushViewerModule>();
        }

        protected override Task OnDocumentActivated(IDocumentContext<ViewerViewModel> activeDocument)
        {
            _engine = activeDocument.Context.Runner.Engine;
            _viewerModule = activeDocument.Context.Modules.FirstOfTypeOrDefault<IBrushViewerModule>();
            _filteringContext = activeDocument as IDocumentContext<IComponentFilter>;

            if (activeDocument is IDocumentContext<IGlyphData> dataContext)
                Items = new[] { dataContext.Context };
            else
                Items = _engine.Root.Components;
            
            //IBrush previousBrush = _viewerModule.Brush;
            //IPaint previousPaint = _viewerModule.Paint;

            HandleSelection(_viewerModule.Canvas);
            SelectedBrush = _brushes.FirstOrDefault();

            //TODO: Handle selection binding
            //SelectedBrush = _brushes.FirstOrDefault(x => x == previousBrush);
            //SelectedPaint = SelectedBrush?.Paints.FirstOrDefault(x => x.Paint == previousPaint);

            _viewerModule.ApplyEnded += OnBrushApplyEnded;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            HandleSelection(null);
            Items = null;

            _engine = null;
            _viewerModule = null;
            _filteringContext = null;

            return Task.CompletedTask;
        }

        private void OnBrushApplyEnded(object sender, EventArgs e)
        {
            EventAggregator.PublishAsync(new DirtyMessage(CurrentDocument, SelectedCanvas)).Wait();
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object model, Func<object, ITreeViewItemModel> dataConverter, Action<ITreeViewItemModel> itemDisposer)
        {
            switch (model)
            {
                case IGlyphData data:
                    return _dataTreeItemBuilder.Build(data, dataConverter, itemDisposer);
                case IGlyphComponent component:
                    return _componentTreeItemBuilder.Build(component, dataConverter, itemDisposer);
                default:
                    throw new NotSupportedException();
            }
        }

        bool ITreeContext.BaseFilter(object model)
        {
            IGlyphComponent componentToFilter;
            switch (model)
            {
                case IGlyphData data:
                    componentToFilter = data.BindedObject;
                    break;
                case IGlyphComponent component:
                    componentToFilter = component;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return GetAllBrushesForType(model).Any(brush => brush.IsValidForCanvas(model))
                   && _filteringContext.Context.Filter(componentToFilter);
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
            HandleSelection(message.Item);
            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
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