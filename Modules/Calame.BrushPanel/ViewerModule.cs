using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Viewer;
using Calame.Viewer.Modules.Base;
using Fingear.Interactives;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Tools.Brushing;
using Glyph.Tools.Brushing.Controllers;
using Glyph.Tools.Brushing.Space;
using MahApps.Metro.IconPacks;
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.BrushPanel
{
    [Export(typeof(IViewerModuleSource))]
    public class EngineViewerModuleSource : IViewerModuleSource
    {
        public bool IsValidForDocument(IDocumentContext documentContext) => !(documentContext is IDocumentContext<IGlyphData>);
        public IViewerModule CreateInstance() => new ViewerModule<IGlyphComponent, EngineCursorBrushController>();
    }

    [Export(typeof(IViewerModuleSource))]
    public class DataViewerModuleSource : IViewerModuleSource
    {
        public bool IsValidForDocument(IDocumentContext documentContext) => documentContext is IDocumentContext<IGlyphData>;
        public IViewerModule CreateInstance() => new ViewerModule<IGlyphData, DataCursorBrushController>();
    }

    public interface IBrushViewerModule : IViewerModule
    {
        object Canvas { get; set; }
        IBrush Brush { get; set; }
        IPaint Paint { get; set; }
    }

    public class ViewerModule<TCanvas, TBrushController> : ViewerModuleBase, IBrushViewerModule, IViewerInteractiveMode
        where TBrushController : SimpleCursorBrushControllerBase<TCanvas, IPaint>
    {
        private GlyphObject _root;
        private TBrushController _brushController;

        public TCanvas Canvas
        {
            get => _brushController.Canvas;
            set => _brushController.Canvas = value;
        }
        
        public IBrush<TCanvas, ISpaceBrushArgs, IPaint> Brush
        {
            get => _brushController.Brush;
            set => _brushController.Brush = value;
        }
        
        public IPaint Paint
        {
            get => _brushController.Paint;
            set => _brushController.Paint = value;
        }

        object IBrushViewerModule.Canvas
        {
            get => Canvas;
            set => Canvas = (TCanvas)value;
        }

        IBrush IBrushViewerModule.Brush
        {
            get => Brush;
            set => Brush = (IBrush<TCanvas, ISpaceBrushArgs, IPaint>)value;
        }
        
        private IInteractive _interactive;
        public IInteractive Interactive
        {
            get => _interactive;
            private set => this.SetValue(ref _interactive, value);
        }
        
        public string Name => "Brush";
        public object IconId => PackIconMaterialKind.Brush;
        Cursor IViewerInteractiveMode.Cursor => Cursors.Pen;
        bool IViewerInteractiveMode.UseFreeCamera => true;
        
        protected override void ConnectRunner()
        {
            _root = Model.EditorRoot.Add<GlyphObject>();

            Interactive = _root.Add<InteractiveRoot>().Interactive;
            Model.AddInteractiveMode(this);

            _brushController = _root.Add<TBrushController>();
            _brushController.Input = InputSystem.Instance.Mouse[MouseButton.Left];
            _brushController.RaycastClient = Model.Client;
        }

        protected override void DisconnectRunner()
        {
            Model.RemoveInteractiveMode(this);
            Model.EditorRoot.RemoveAndDispose(_root);

            _brushController = null;
            _root = null;
        }
    }
}