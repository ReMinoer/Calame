using System;
using Caliburn.Micro;
using Diese.Collections;
using Fingear.Controls;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.Viewer
{
    public class ViewerViewModel : PropertyChangedBase, IHandle<ISelection<IGlyphComponent>>, IDisposable
    {
        private readonly IViewerViewModelOwner _owner;
        private readonly IEventAggregator _eventAggregator;
        
        private GlyphWpfRunner _runner;
        private IBoxedComponent _boxedSelection;
        private ShapedObjectSelector _shapedObjectSelector;
        private AreaComponentRenderer _selectionRenderer;

        public IWpfGlyphClient Client { get; private set; }
        public FillView EditorView { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        public GlyphObject EditorRoot { get; private set; }

        public IBoxedComponent BoxedSelection
        {
            get => _boxedSelection;
            private set
            {
                if (this.SetValue(ref _boxedSelection, value))
                {
                    if (_selectionRenderer != null)
                    {
                        EditorRoot.Remove(_selectionRenderer);
                        _selectionRenderer.Dispose();
                        _selectionRenderer = null;
                    }

                    _boxedSelection = value;

                    if (_boxedSelection != null)
                    {
                        _selectionRenderer = new AreaComponentRenderer(_boxedSelection, Runner.Engine.Injector.Resolve<Func<GraphicsDevice>>()) { Name = "Selection Renderer", Color = Color.Purple * 0.5f, DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera };
                        EditorRoot.Add(_selectionRenderer);
                    }
                }
            }
        }

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    GlyphEngine engine = _runner.Engine;

                    engine.Root.RemoveAndDispose(EditorRoot);
                    EditorRoot = null;
                }

                _runner = value;

                if (_runner != null)
                {
                    GlyphEngine engine = _runner.Engine;

                    EditorRoot = engine.Root.Add<GlyphObject>();
                    EditorRoot.Name = "Editor Root";
                    EditorRoot.Add<SceneNode>().MakesRoot();

                    EditorView = engine.Root.Add<FillView>();
                    EditorView.Name = "Editor View";
                    EditorView.ParentView = engine.RootView;
                    EditorView.DrawClientFilter = new ExcludingFilter<IDrawClient>();
                    
                    EditorCamera = EditorRoot.Add<FreeCamera>();
                    ConnectRunner();
                    EditorCamera.View = EditorView;

                    _shapedObjectSelector = EditorRoot.Add<ShapedObjectSelector>();
                    _shapedObjectSelector.Control = new HybridControl<System.Numerics.Vector2>("Pointer")
                    {
                        TriggerControl = new Control(InputSystem.Instance.Mouse[MouseButton.Left]),
                        ValueControl = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, engine.RootView, new ReadOnlySceneNodeDelegate(EditorCamera.GetSceneNode), engine.ProjectionManager)
                    };
                    _shapedObjectSelector.HandleInputs = true;
                    _shapedObjectSelector.SelectionChanged += ShapedObjectSelectorOnSelectionChanged;
                }
                else
                    ConnectRunner();

                RunnerChanged?.Invoke(this, Runner);
                Activate();
            }
        }

        public event EventHandler<GlyphWpfRunner> RunnerChanged;
        public event EventHandler<IBoxedComponent> SelectionChanged;

        public ViewerViewModel(IViewerViewModelOwner owner, IEventAggregator eventAggregator)
        {
            _owner = owner;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public void ConnectView(IViewerView view)
        {
            Client = view?.Client;

            ConnectRunner();

            _owner.Activated += OnActivated;
            _owner.Deactivated += OnDeactivated;
        }

        private void ConnectRunner()
        {
            if (Client == null || Runner == null)
                return;

            Client.Runner = Runner;
            EditorCamera.Client = Client;
            Runner.Engine.FocusedClient = Client;
        }

        private void ShapedObjectSelectorOnSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (Runner.Engine.FocusedClient == Client)
            {
                BoxedSelection = boxedComponent;
                _eventAggregator.PublishOnUIThread(Selection.New(_boxedSelection));
                SelectionChanged?.Invoke(this, boxedComponent);
            }
        }

        private void Activate()
        {
            if (Runner?.Engine != null)
                Runner.Engine.FocusedClient = Client;

            _eventAggregator.PublishOnUIThread(new DocumentContext<GlyphEngine>(_runner?.Engine));
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => Activate();

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            if (Runner?.Engine != null && Runner.Engine.FocusedClient == Client)
                Runner.Engine.FocusedClient = null;
        }

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message)
        {
            if (Runner.Engine.FocusedClient == Client && message.Item is IBoxedComponent boxedComponent)
                BoxedSelection = boxedComponent;
        }

        public void Dispose()
        {
            _owner.Activated -= OnActivated;
            _owner.Deactivated -= OnDeactivated;

            _eventAggregator.Unsubscribe(this);

            Runner?.Dispose();
            Runner = null;
        }
    }
}