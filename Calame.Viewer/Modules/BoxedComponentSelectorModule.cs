using System.Threading;
using System.Threading.Tasks;
using Calame.Viewer.Modules.Base;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Resolvers;
using Glyph.Engine;
using Glyph.Messaging;
using Glyph.Tools;
using Niddle;

namespace Calame.Viewer.Modules
{
    public class BoxedComponentSelectorModule : ViewerModuleBase, IHandle<IDocumentContext<ViewerViewModel>>
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
        
        public BoxedComponentSelectorModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        
        protected override void ConnectRunner()
        {
            GlyphEngine engine = Model.Runner.Engine;

            _root = Model.EditorModeRoot.Add<GlyphObject>();

            _shapedObjectSelector = engine.Resolver.WithLink<ISubscribableRouter, ISubscribableRouter>(ResolverScope.Global).Resolve<ShapedObjectSelector>();
            _shapedObjectSelector.Filter = ComponentFilter;

            _root.Add(_shapedObjectSelector);

            _eventAggregator.SubscribeOnUI(this);
            _shapedObjectSelector.SelectionChanged += OnShapedObjectSelectorSelectionChanged;
        }

        protected override void DisconnectRunner()
        {
            _shapedObjectSelector.SelectionChanged -= OnShapedObjectSelectorSelectionChanged;
            _eventAggregator.Unsubscribe(this);

            Model.EditorModeRoot.RemoveAndDispose(_root);

            _shapedObjectSelector = null;
            _root = null;
        }

        private bool ComponentFilter(IGlyphComponent glyphComponent)
        {
            return _filteringContext?.Context.Filter(glyphComponent) ?? true;
        }

        private async void OnShapedObjectSelectorSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (Model.Runner.Engine.FocusedClient != Model.Client)
                return;

            SelectedComponent = boxedComponent;
            await _eventAggregator.PublishAsync(new SelectionRequest<IBoxedComponent>(_currentDocument, SelectedComponent));
        }

        Task IHandle<IDocumentContext<ViewerViewModel>>.HandleAsync(IDocumentContext<ViewerViewModel> message, CancellationToken cancellationToken)
        {
            _currentDocument = message;
            _filteringContext = message as IDocumentContext<IComponentFilter>;

            return Task.CompletedTask;
        }
    }
}