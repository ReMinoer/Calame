using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.ViewGraph.Graph;
using Calame.ViewGraph.Utils;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using GraphShape.Controls;

namespace Calame.ViewGraph.ViewModels
{
    [Export(typeof(ViewGraphViewModel))]
    public sealed class ViewGraphViewModel : CalameTool<IDocumentContext<IViewsContext>>,
        IHandle<ISelectionSpread<IGlyphComponent>>,
        IHandle<ISelectionSpread<IGlyphData>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Bottom;

        public IIconProvider IconProvider { get; }
        public IIconDescriptor<IGlyphComponent> IconDescriptor { get; }

        private IViewsContext _viewsContext;
        public IViewsContext ViewsContext
        {
            get => _viewsContext;
            set => Set(ref _viewsContext, value);
        }

        private ViewGraphGraph _graph;
        public ViewGraphGraph Graph
        {
            get => _graph;
            private set => Set(ref _graph, value);
        }

        private ISelectionContext<IGlyphComponent> _selectionContext;
        private ICommand _selectionCommand;
        public ICommand SelectionCommand
        {
            get => _selectionCommand;
            private set => Set(ref _selectionCommand, value);
        }

        private IGlyphComponent _selection;
        public IGlyphComponent Selection
        {
            get => _selection;
            set
            {
                if (!SetValue(ref _selection, value))
                    return;

                SelectVertex();
                _selectionContext.SelectAsync(_selection).Wait();
            }
        }
        
        public ITransition Transition { get; } = new DirectTransition();

        protected override object IconKey => CalameIconKey.ViewGraph;

        [ImportingConstructor]
        public ViewGraphViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "View Graph";
            
            IconProvider = iconProvider;
            IconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();
        }

        protected override Task OnDocumentActivated(IDocumentContext<IViewsContext> activeDocument)
        {
            UnregisterViewsContext();

            _selection = null;

            _selectionContext = activeDocument.GetSelectionContext<IGlyphComponent>();
            SelectionCommand = _selectionContext.GetSelectionCommand();

            ViewsContext = activeDocument.Context;
            RefreshGraph();

            RegisterViewsContext();

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            UnregisterViewsContext();

            _selection = null;

            Graph.Clear();
            ViewsContext = null;

            _selectionCommand = null;
            _selectionContext = null;

            return Task.CompletedTask;
        }

        private void RegisterViewsContext()
        {
            if (ViewsContext is INotifyPropertyChanged propertyChanged)
                propertyChanged.PropertyChanged += OnViewContextChanged;

            if (ViewsContext?.Views is INotifyCollectionChanged collectionChanged)
                collectionChanged.CollectionChanged += OnViewCollectionChanged;
        }

        private void UnregisterViewsContext()
        {
            if (ViewsContext?.Views is INotifyCollectionChanged collectionChanged)
                collectionChanged.CollectionChanged -= OnViewCollectionChanged;

            if (ViewsContext is INotifyPropertyChanged propertyChanged)
                propertyChanged.PropertyChanged -= OnViewContextChanged;
        }

        private void OnViewContextChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IViewsContext.Views))
                RefreshGraph();
        }

        private void OnViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshGraph();
        }

        private void OnDirtiedVertex(object sender, EventArgs e)
        {
            RefreshGraph();
        }

        private void RefreshGraph()
        {
            if (Graph != null)
            {
                foreach (ViewGraphVertex vertex in Graph.Vertices)
                {
                    vertex.Dirtied -= OnDirtiedVertex;
                    vertex.Dispose();
                }
            }

            var graph = new ViewGraphGraph();
            var sceneVertices = new Dictionary<ISceneNode, ViewGraphVertex>();

            foreach (IView view in ViewsContext.Views)
            {
                var viewVertex = new ViewGraphVertex(view);
                AddVertex(viewVertex);

                ISceneNode viewSceneRoot = view.GetSceneNode().RootNode();
                if (viewSceneRoot != null)
                {
                    if (!sceneVertices.TryGetValue(viewSceneRoot, out ViewGraphVertex viewSceneVertex))
                    {
                        ISceneNode representedViewSceneNode = viewSceneRoot.AndAllChildNodes().FirstOrDefault(_selectionContext.CanSelect) ?? viewSceneRoot;

                        viewSceneVertex = new ViewGraphVertex(representedViewSceneNode, viewSceneRoot);
                        AddVertex(viewSceneVertex);

                        sceneVertices.Add(viewSceneRoot, viewSceneVertex);
                    }

                    AddEdge(viewSceneVertex, viewVertex);
                }

                ISceneNode cameraSceneRoot = view.Camera.GetSceneNode().RootNode();
                if (cameraSceneRoot != null)
                {
                    if (!sceneVertices.TryGetValue(cameraSceneRoot, out ViewGraphVertex cameraSceneVertex))
                    {
                        ISceneNode representedCameraSceneNode = cameraSceneRoot.AndAllChildNodes().FirstOrDefault(_selectionContext.CanSelect) ?? cameraSceneRoot;

                        cameraSceneVertex = new ViewGraphVertex(representedCameraSceneNode, cameraSceneRoot);
                        AddVertex(cameraSceneVertex);

                        sceneVertices.Add(cameraSceneRoot, cameraSceneVertex);
                    }

                    AddEdge(viewVertex, cameraSceneVertex);
                }
            }

            Graph = graph;

            void AddVertex(ViewGraphVertex vertex)
            {
                graph.AddVertex(vertex);
                vertex.Selected += OnNodeSelected;
                vertex.Dirtied += OnDirtiedVertex;
            }

            void AddEdge(ViewGraphVertex first, ViewGraphVertex second)
            {
                graph.AddEdge(new ViewGraphEdge(first, second));
            }
        }

        Task IHandle<ISelectionSpread<IGlyphComponent>>.HandleAsync(ISelectionSpread<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item);
            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item?.BindedObject);
            return Task.CompletedTask;
        }

        private void HandleSelection(IGlyphComponent component)
        {
            SetValue(ref _selection, component, nameof(Selection));
            SelectVertex();
        }

        private void SelectVertex()
        {
            ViewGraphVertex selectedVertex = Graph.Vertices.FirstOrDefault(x => x.Data == _selection);
            if (selectedVertex != null)
            {
                selectedVertex.IsSelected = true;
            }
            else
            {
                foreach (ViewGraphVertex vertex in Graph.Vertices)
                    vertex.IsSelected = false;
            }
        }

        private void OnNodeSelected(object sender, EventArgs e)
        {
            foreach (ViewGraphVertex vertex in Graph.Vertices.Where(x => x != sender))
                vertex.IsSelected = false;
        }
    }
}