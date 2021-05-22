using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Icons;
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
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.BrushPanel
{
    [Export(typeof(IViewerModuleSource))]
    public class EngineViewerModuleSource : IViewerModuleSource
    {
        public bool IsValidForDocument(IDocumentContext documentContext) => !(documentContext is IDocumentContext<IRootDataContext>);
        public IViewerModule CreateInstance() => new ViewerModule<IGlyphComponent, EngineCursorBrushController>();
    }

    [Export(typeof(IViewerModuleSource))]
    public class DataViewerModuleSource : IViewerModuleSource
    {
        public bool IsValidForDocument(IDocumentContext documentContext) => documentContext is IDocumentContext<IRootDataContext>;
        public IViewerModule CreateInstance() => new ViewerModule<IGlyphData, DataCursorBrushController>();
    }

    public interface IBrushViewerModule : IViewerModule, IViewerInteractiveMode, IBrushController<object>
    {
        IBrush Brush { get; set; }
        IPaint Paint { get; set; }
    }

    public class ViewerModule<TCanvas, TBrushController> : ViewerModuleBase, IBrushViewerModule
        where TCanvas : class
        where TBrushController : SimpleCursorBrushControllerBase<TCanvas, IBrush<TCanvas, ISpaceBrushArgs, IPaint>, IPaint>
    {
        private GlyphObject _root;
        private TBrushController _brushController;

        public bool Enabled { get; set; }

        public TCanvas Canvas
        {
            get => _brushController?.Canvas;
            set
            {
                if (_brushController == null)
                    return;

                _brushController.Canvas = value;
            }
        }

        public IBrush<TCanvas, ISpaceBrushArgs, IPaint> Brush
        {
            get => _brushController?.Brush;
            set
            {
                if (_brushController == null)
                    return;

                _brushController.Brush = value;
            }
        }
        
        public IPaint Paint
        {
            get => _brushController?.Paint;
            set
            {
                if (_brushController == null)
                    return;

                _brushController.Paint = value;
            }
        }

        object IBrushController<object>.Canvas
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

        public bool ApplyingBrush => _brushController.ApplyingBrush;

        public event EventHandler ApplyStarted;
        public event EventHandler ApplyCancelled;
        public event EventHandler ApplyEnded;

        public string Name => "Brush";
        public object IconKey => CalameIconKey.BrushMode;
        Cursor IViewerInteractiveMode.Cursor => Cursors.Pen;
        bool IViewerInteractiveMode.UseFreeCamera => true;
        bool IViewerInteractiveMode.IsUserMode => false;

        protected override void ConnectModel() => Model.AddInteractiveMode(this);
        protected override void DisconnectModel() => Model.RemoveInteractiveMode(this);

        protected override void ConnectRunner()
        {
            _root = Model.EditorRoot.Add<GlyphObject>();

            Interactive = _root.Add<InteractiveRoot>().Interactive;

            _brushController = _root.Add<TBrushController>();
            _brushController.Input = InputSystem.Instance.Mouse[MouseButton.Left];
            _brushController.RaycastClient = Model.Client;

            _brushController.ApplyStarted += OnApplyStarted;
            _brushController.ApplyCancelled += OnApplyCancelled;
            _brushController.ApplyEnded += OnApplyEnded;
        }

        protected override void DisconnectRunner()
        {
            _brushController.ApplyEnded -= OnApplyEnded;
            _brushController.ApplyCancelled -= OnApplyCancelled;
            _brushController.ApplyStarted -= OnApplyStarted;

            Model.EditorRoot.RemoveAndDispose(_root);

            _brushController = null;
            _root = null;
        }

        void IViewerInteractiveMode.OnSelected()
        {
            _brushController.Enabled = true;
            _brushController.Visible = true;
        }

        void IViewerInteractiveMode.OnUnselected()
        {
            _brushController.Enabled = false;
            _brushController.Visible = false;
        }

        private void OnApplyStarted(object sender, EventArgs e) => ApplyStarted?.Invoke(this, e);
        private void OnApplyCancelled(object sender, EventArgs e) => ApplyCancelled?.Invoke(this, e);
        private void OnApplyEnded(object sender, EventArgs e) => ApplyEnded?.Invoke(this, e);
    }
}