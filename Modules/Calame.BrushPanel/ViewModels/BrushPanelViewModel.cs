using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
using Glyph.Engine;

namespace Calame.BrushPanel.ViewModels
{
    [Export(typeof(BrushPanelViewModel))]
    public sealed class BrushPanelViewModel : HandleTool, ITreeContext, IHandle<IDocumentContext<ViewerViewModel>>, IHandle<ISelection<IGlyphComponent>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;
        
        private ViewerModule _viewerModule;
        private readonly IBrushViewModel[] _allBrushes;
        private readonly ObservableCollection<IBrushViewModel> _brushes;
        public IEnumerable<IBrushViewModel> Brushes => _brushes;
        
        private GlyphEngine _engine;
        public GlyphEngine Engine
        {
            get => _engine;
            private set => SetValue(ref _engine, value);
        }

        private IGlyphComponent _selectedCanvas;
        public IGlyphComponent SelectedCanvas
        {
            get => _selectedCanvas;
            set
            {
                if (SetValue(ref _selectedCanvas, value))
                    OnCanvasChanged();
 
                EventAggregator.PublishOnUIThread(new Selection<IGlyphComponent>(_selectedCanvas));
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
        public BrushPanelViewModel(IShell shell, IEventAggregator eventAggregator, CompositionContainer compositionContainer, [ImportMany] IBrushViewModel[] allBrushes)
            : base(eventAggregator)
        {
            DisplayName = "Brush Panel";

            SelectBrushCommand = new RelayCommand(x => SelectedBrush = (IBrushViewModel)x);
            SelectPaintCommand = new RelayCommand(x => SelectedPaint = (IPaintViewModel)x);

            _allBrushes = allBrushes;
            _brushes = new ObservableCollection<IBrushViewModel>();

            if (shell.ActiveItem is IDocumentContext<ViewerViewModel> documentContext)
                _viewerModule = documentContext.Context.Modules.FirstOfTypeOrDefault<ViewerModule>();
        }

        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data)
        {
            return new TreeViewItemModel<IGlyphComponent>(
                this,
                (IGlyphComponent)data,
                x => x.Name,
                x => new EnumerableReadOnlyObservableList<object>(x.Components),
                nameof(IGlyphComponent.Name),
                nameof(IGlyphComponent.Components));
        }
        
        bool ITreeContext.BaseFilter(object data)
        {
            return _allBrushes.Any(brush => IsValidBrushForCanvas(data, brush));
        }

        static private bool IsValidBrushForCanvas(object canvas, IBrushViewModel brush)
        {
            return brush.CanvasType.IsInstanceOfType(canvas);
        }

        private void OnCanvasChanged()
        {
            SelectedBrush = null;

            _brushes.Clear();

            if (SelectedCanvas != null)
                foreach (IBrushViewModel brush in _allBrushes.Where(x => IsValidBrushForCanvas(SelectedCanvas, x)))
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

        void IHandle<IDocumentContext<ViewerViewModel>>.Handle(IDocumentContext<ViewerViewModel> message)
        {
            Engine = message.Context.Runner.Engine;

            _viewerModule = message.Context.Modules.FirstOfTypeOrDefault<ViewerModule>();

            SelectedCanvas = _viewerModule.Canvas;
            SelectedBrush = _brushes.FirstOrDefault(x => x.Brush == _viewerModule.Brush);
            SelectedPaint = SelectedBrush?.Paints.FirstOrDefault(x => x.Paint == _viewerModule.Paint);
        }

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message)
        {
            if (SetValue(ref _selectedCanvas, message.Item, nameof(SelectedCanvas)))
                OnCanvasChanged();
        }
    }
}