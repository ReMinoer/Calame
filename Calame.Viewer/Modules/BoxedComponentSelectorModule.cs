using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calame.DocumentContexts;
using Calame.Viewer.Modules.Base;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Resolvers;
using Glyph.Engine;
using Glyph.Messaging;
using Glyph.Tools;
using Niddle;
using Stave;

namespace Calame.Viewer.Modules
{
    public class BoxedComponentSelectorModule : ViewerModuleBase
    {
        private ISelectionContext<IGlyphComponent> _selectionContext;

        private GlyphObject _root;
        private ShapedObjectSelector _shapedObjectSelector;

        private IBoxedComponent _selectedComponent;
        public IBoxedComponent SelectedComponent
        {
            get => _selectedComponent;
            private set => this.SetValue(ref _selectedComponent, value);
        }
        
        public BoxedComponentSelectorModule(ISelectionContext<IGlyphComponent> selectionContext)
        {
            _selectionContext = selectionContext;
        }

        protected override void ConnectModel() {}
        protected override void DisconnectModel() {}

        protected override void ConnectRunner()
        {
            GlyphEngine engine = Model.Runner.Engine;

            _root = Model.EditorModeRoot.Add<GlyphObject>();

            _shapedObjectSelector = engine.Resolver.WithLink<ISubscribableRouter, ISubscribableRouter>(ResolverScope.Global).Resolve<ShapedObjectSelector>();
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