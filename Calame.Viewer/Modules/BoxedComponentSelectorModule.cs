using System.Linq;
using Calame.DocumentContexts;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Engine;
using Glyph.Tools;
using Niddle;
using Stave;

namespace Calame.Viewer.Modules
{
    public class BoxedComponentSelectorModule : SelectionHandlerModuleBase
    {
        private readonly ISelectionContext<IGlyphComponent> _selectionContext;

        private GlyphObject _root;
        private ShapedObjectSelector _shapedObjectSelector;

        private IBoxedComponent _selectedComponent;
        public IBoxedComponent SelectedComponent
        {
            get => _selectedComponent;
            private set => this.SetValue(ref _selectedComponent, value);
        }
        
        public BoxedComponentSelectorModule(IEventAggregator eventAggregator, ISelectionContext<IGlyphComponent> selectionContext)
             : base(eventAggregator)
        {
            _selectionContext = selectionContext;
        }

        protected override void ConnectRunner()
        {
            base.ConnectRunner();

            GlyphEngine engine = Model.Runner.Engine;

            _root = Model.EditorModeRoot.Add<GlyphObject>();

            _shapedObjectSelector = engine.Resolver.WithInstance<IGlyphComponent>(Model.UserRoot).Resolve<ShapedObjectSelector>();
            _shapedObjectSelector.Filter = ComponentFilter;

            _root.Add(_shapedObjectSelector);
            
            _shapedObjectSelector.SelectionChanged += OnShapedObjectSelectorSelectionChanged;
        }

        protected override void DisconnectRunner()
        {
            _shapedObjectSelector.SelectionChanged -= OnShapedObjectSelectorSelectionChanged;

            Model.EditorModeRoot.RemoveAndDispose(_root);

            _shapedObjectSelector = null;
            _root = null;

            base.DisconnectRunner();
        }

        protected override void HandleComponent(IGlyphComponent selection)
        {
            _shapedObjectSelector.Selection = selection as IBoxedComponent;
        }

        protected override void ReleaseComponent(IGlyphComponent selection)
        {
            _shapedObjectSelector.Selection = null;
        }

        private bool ComponentFilter(IGlyphComponent component)
        {
            return _selectionContext?.CanSelect(component) ?? true;
        }

        private async void OnShapedObjectSelectorSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (Model.Runner.Engine.FocusedClient != Model.Client)
                return;

            SelectedComponent = boxedComponent?.AllParents().OfType<IBoxedComponent>().First(x => x.Components.AnyOfType<ISceneNodeComponent>());
            await _selectionContext.SelectAsync(SelectedComponent);
        }
    }
}