using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Fingear;
using Fingear.Controls;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Core.Resolvers;
using Glyph.Engine;
using Glyph.Messaging;
using Glyph.Tools;
using MahApps.Metro.IconPacks;
using Niddle;
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.Viewer.Modules
{
    [Export(typeof(IViewerModuleSource))]
    public class BoxedComponentSelectorModuleSource : IViewerModuleSource
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public BoxedComponentSelectorModuleSource(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool IsValidForDocument(IDocumentContext documentContext) => true;
        public IViewerModule CreateInstance() => new BoxedComponentSelectorModule(_eventAggregator);
    }

    public class BoxedComponentSelectorModule : ViewerModuleBase, IViewerInteractiveMode, IHandle<IDocumentContext<ViewerViewModel>>
    {
        private readonly IEventAggregator _eventAggregator;
        private IDocumentContext _currentDocument;
        private IDocumentContext<IComponentFilter> _filteringContext;

        private GlyphObject _root;
        private ShapedObjectSelector _shapedObjectSelector;

        private IBoxedComponent _selectedComponent;
        public IBoxedComponent SelectedComponent
        {
            get => _selectedComponent;
            private set => this.SetValue(ref _selectedComponent, value);
        }
        
        private IInteractive _interactive;

        public IInteractive Interactive
        {
            get => _interactive;
            private set => this.SetValue(ref _interactive, value);
        }
        
        public string Name => "Editor";
        public object IconId => PackIconMaterialKind.CursorDefaultOutline;
        Cursor IViewerInteractiveMode.Cursor => Cursors.Cross;
        bool IViewerInteractiveMode.UseFreeCamera => true;
        
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
            Model.AddInteractiveMode(this);

            _shapedObjectSelector = engine.Resolver.WithLink<ISubscribableRouter, ISubscribableRouter>(ResolverScope.Global).Resolve<ShapedObjectSelector>();
            _shapedObjectSelector.Filter = ComponentFilter;
            _shapedObjectSelector.Control = new HybridControl<System.Numerics.Vector2>("Pointer")
            {
                TriggerControl = new Control(InputSystem.Instance.Mouse[MouseButton.Left]),
                ValueControl = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, engine.RootView, new ReadOnlySceneNodeDelegate(Model.EditorCamera.GetSceneNode), engine.ProjectionManager)
                {
                    RaycastClient = Model.Client
                }
            };

            _root.Add(_shapedObjectSelector);

            _eventAggregator.Subscribe(this);
            _shapedObjectSelector.SelectionChanged += OnShapedObjectSelectorSelectionChanged;
        }

        protected override void DisconnectRunner()
        {
            _shapedObjectSelector.SelectionChanged -= OnShapedObjectSelectorSelectionChanged;
            _eventAggregator.Unsubscribe(this);
            
            Model.RemoveInteractiveMode(this);
            _root.Dispose();

            _shapedObjectSelector = null;
            _root = null;
        }

        private bool ComponentFilter(IGlyphComponent glyphComponent)
        {
            return _filteringContext?.Context.Filter(glyphComponent) ?? true;
        }

        private void OnShapedObjectSelectorSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (Model.Runner.Engine.FocusedClient != Model.Client)
                return;

            SelectedComponent = boxedComponent;
            _eventAggregator.PublishOnUIThread(new SelectionRequest<IBoxedComponent>(_currentDocument, SelectedComponent));
        }

        public void Handle(IDocumentContext<ViewerViewModel> message)
        {
            _currentDocument = message;
            _filteringContext = message as IDocumentContext<IComponentFilter>;
        }
    }
}