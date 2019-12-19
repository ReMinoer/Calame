using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Viewer;
using Calame.Viewer.Modules.Base;
using Fingear;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Tools.Brushing;
using Glyph.Tools.Brushing.Controllers;
using Glyph.Tools.Brushing.Space;
using MahApps.Metro.IconPacks;
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.BrushPanel
{
    [Export(typeof(IViewerModule))]
    public class ViewerModule : ViewerModuleBase, IViewerMode
    {
        private GlyphObject _root;
        private CursorBrushController<IGlyphComponent, IPaint> _brushController;
        
        public IGlyphComponent Canvas
        {
            get => _brushController.Canvas;
            set => _brushController.Canvas = value;
        }
        
        public IBrush<IGlyphComponent, ISpaceBrushArgs, IPaint> Brush
        {
            get => _brushController.Brush;
            set => _brushController.Brush = value;
        }
        
        public IPaint Paint
        {
            get => _brushController.Paint;
            set => _brushController.Paint = value;
        }
        
        private IInteractive _interactive;
        public IInteractive Interactive
        {
            get => _interactive;
            private set => this.SetValue(ref _interactive, value);
        }
        
        public string Name => "Brush";
        public object IconId => PackIconMaterialKind.Brush;
        Cursor IViewerMode.Cursor => Cursors.Pen;
        bool IViewerMode.UseFreeCamera => true;
        
        protected override void ConnectRunner()
        {
            _root = Model.EditorRoot.Add<GlyphObject>();

            Interactive = _root.Add<InteractiveRoot>().Interactive;
            Model.InteractiveToggle.Add(Interactive);
            Model.InteractiveModules.Add(this);

            _brushController = _root.Add<CursorBrushController<IGlyphComponent, IPaint>>();
            _brushController.Input = InputSystem.Instance.Mouse[MouseButton.Left];
            _brushController.RaycastClient = Model.Client;
        }

        protected override void DisconnectRunner()
        {
            Model.InteractiveModules.Remove(this);
            Model.EditorRoot.Remove(_root);
            _root.Dispose();

            _brushController = null;
            _root = null;
        }
    }
}