using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Fingear;
using Fingear.Controls;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Tools;
using MahApps.Metro.IconPacks;
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.Viewer.Modules
{
    [Export(typeof(IViewerModule))]
    public class BoxedComponentSelectorModule : ViewerModuleBase, IViewerMode, IHandle<IDocumentContext>
    {
        private readonly IEventAggregator _eventAggregator;
        private IDocumentContext _currentDocument;

        private GlyphObject _root;
        private ShapedObjectSelector _shapedObjectSelector;

        private IBoxedComponent _selectedComponent;
        public IBoxedComponent SelectedComponent
        {
            get => _selectedComponent;
            private set
            {
                if (this.SetValue(ref _selectedComponent, value))
                    SelectionChanged?.Invoke(this, value);
            }
        }
        
        private IInteractive _interactive;
        public IInteractive Interactive
        {
            get => _interactive;
            private set => this.SetValue(ref _interactive, value);
        }
        
        public string Name => "Editor";
        public object IconId => PackIconMaterialKind.CursorDefaultOutline;
        Cursor IViewerMode.Cursor => Cursors.Cross;
        bool IViewerMode.UseFreeCamera => true;

        public event EventHandler<IBoxedComponent> SelectionChanged;
        
        [ImportingConstructor]
        public BoxedComponentSelectorModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        
        protected override void ConnectRunner()
        {
            GlyphEngine engine = Model.Runner.Engine;

            _root = Model.EditorRoot.Add<GlyphObject>();

            Interactive = _root.Add<InteractiveRoot>().Interactive;
            Model.InteractiveToggle.Add(Interactive);
            Model.InteractiveModules.Add(this);

            _shapedObjectSelector = _root.Add<ShapedObjectSelector>();
            _shapedObjectSelector.Control = new HybridControl<System.Numerics.Vector2>("Pointer")
            {
                TriggerControl = new Control(InputSystem.Instance.Mouse[MouseButton.Left]),
                ValueControl = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, engine.RootView, new ReadOnlySceneNodeDelegate(Model.EditorCamera.GetSceneNode), engine.ProjectionManager)
                {
                    RaycastClient = Model.Client
                }
            };

            _eventAggregator.Subscribe(this);
            _shapedObjectSelector.SelectionChanged += OnShapedObjectSelectorSelectionChanged;
        }

        protected override void DisconnectRunner()
        {
            _shapedObjectSelector.SelectionChanged -= OnShapedObjectSelectorSelectionChanged;
            _eventAggregator.Unsubscribe(this);
            
            Model.InteractiveModules.Remove(this);
            Model.EditorRoot.Remove(_root);
            _root.Dispose();

            _shapedObjectSelector = null;
            _root = null;
        }

        private void OnShapedObjectSelectorSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (Model.Runner.Engine.FocusedClient != Model.Client)
                return;

            SelectedComponent = boxedComponent;
            _eventAggregator.PublishOnUIThread(new SelectionRequest<IBoxedComponent>(_currentDocument, SelectedComponent));
        }

        public void Handle(IDocumentContext message)
        {
            _currentDocument = message;
        }
    }
}