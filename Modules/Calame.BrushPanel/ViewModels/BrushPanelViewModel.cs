using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Calame.UserControls;
using Calame.Utils;
using Calame.Viewer;
using Caliburn.Micro;
using Diese.Collections;
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
        
        private GlyphEngine _engine;
        private IBrushViewerModule _viewerModule;
        private IDocumentContext<IComponentFilter> _filteringContext;

        private readonly IEngineBrushViewModel[] _allEngineBrushes;
        private readonly IDataBrushViewModel[] _allDataBrushes;
        private readonly ObservableCollection<IBrushViewModel> _brushes;
        public IEnumerable<IBrushViewModel> Brushes => _brushes;

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

                if (SetValue(ref _selectedCanvas, value))
                {
                    OnCanvasChanged();

                    if (_selectedCanvas != null)
                    {
                        switch (_selectedCanvas)
                        {
                            case IGlyphData data:
                                EventAggregator.PublishOnUIThread(new SelectionRequest<IGlyphData>(CurrentDocument, data));
                                break;
                            case IGlyphComponent component:
                                EventAggregator.PublishOnUIThread(new SelectionRequest<IGlyphComponent>(CurrentDocument, component));
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
                                EventAggregator.PublishOnUIThread(new SelectionRequest<IGlyphData>(CurrentDocument, (IGlyphData)null));
                                break;
                            case IGlyphComponent _:
                                EventAggregator.PublishOnUIThread(new SelectionRequest<IGlyphComponent>(CurrentDocument, (IGlyphComponent)null));
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }
                }
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
        public BrushPanelViewModel(IShell shell, IEventAggregator eventAggregator, [ImportMany] IEngineBrushViewModel[] allEngineBrushes, [ImportMany] IDataBrushViewModel[] allDataBrushes)
            : base(shell, eventAggregator)
        {
            DisplayName = "Brush Panel";

            SelectBrushCommand = new RelayCommand(x => SelectedBrush = (IBrushViewModel)x);
            SelectPaintCommand = new RelayCommand(x => SelectedPaint = (IPaintViewModel)x);

            _allEngineBrushes = allEngineBrushes;
            _allDataBrushes = allDataBrushes;
            _brushes = new ObservableCollection<IBrushViewModel>();

            if (shell.ActiveItem is IDocumentContext<ViewerViewModel> documentContext)
                _viewerModule = documentContext.Context.Modules.FirstOfTypeOrDefault<IBrushViewerModule>();
        }

        protected override void OnDocumentActivated(IDocumentContext<ViewerViewModel> activeDocument)
        {
            _engine = activeDocument.Context.Runner.Engine;
            _viewerModule = activeDocument.Context.Modules.FirstOfTypeOrDefault<IBrushViewerModule>();
            _filteringContext = activeDocument as IDocumentContext<IComponentFilter>;

            if (activeDocument is IDocumentContext<IGlyphData> dataContext)
                Items = new[] { dataContext.Context };
            else
                Items = _engine.Root.Components;

            SelectedCanvas = _viewerModule.Canvas;
            SelectedBrush = _brushes.FirstOrDefault(x => x == _viewerModule.Brush);
            SelectedPaint = SelectedBrush?.Paints.FirstOrDefault(x => x.Paint == _viewerModule.Paint);
        }

        protected override void OnDocumentsCleaned()
        {
            _engine = null;
            _viewerModule = null;
            _filteringContext = null;
            _brushes.Clear();

            Items = null;
            SelectedCanvas = null;
            SelectedBrush = null;
            SelectedPaint = null;
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object model)
        {
            switch (model)
            {
                case IGlyphData data:
                    return new TreeViewItemModel<IGlyphData>(
                        this,
                        data,
                        x => x.Name,
                        x => new EnumerableReadOnlyObservableList<object>(x.Children),
                        nameof(IGlyphData.Name),
                        nameof(IGlyphData.Children));
                case IGlyphComponent component:
                    return new TreeViewItemModel<IGlyphComponent>(
                        this,
                        component,
                        x => x.Name,
                        x => new EnumerableReadOnlyObservableList<object>(x.Components),
                        nameof(IGlyphComponent.Name),
                        nameof(IGlyphComponent.Components));
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

        void IHandle<ISelectionSpread<IGlyphComponent>>.Handle(ISelectionSpread<IGlyphComponent> message) => HandleSelection(message.Item);
        void IHandle<ISelectionSpread<IGlyphData>>.Handle(ISelectionSpread<IGlyphData> message) => HandleSelection(message.Item);

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

            SelectedBrush = _brushes.FirstOrDefault();

            _viewerModule.Canvas = SelectedCanvas;
        }

        private void OnBrushChanged()
        {
            SelectedPaint = null;
            _viewerModule.Brush = SelectedBrush;
            SelectedPaint = SelectedBrush?.Paints.FirstOrDefault();
        }

        private void OnPaintChanged()
        {
            _viewerModule.Paint = SelectedPaint?.Paint;
        }
    }
}