using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Calame.SceneViewer.Views;
using Caliburn.Micro;
using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : Document, IDocumentContext<GlyphEngine>, IDisposable
    {
        private readonly IShell _shell;
        private readonly ContentManagerProvider _contentManagerProvider;
        private readonly IEventAggregator _eventAggregator;

        private SceneViewerView _view;
        private Cursor _viewerCursor;
        private GlyphWpfRunner _runner;
        private ShapedObjectSelector _shapedObjectSelector;
        private AreaComponentRenderer _selectionRenderer;
        public ISession Session { get; set; }
        public FreeCamera EditorCamera { get; private set; }
        public Glyph.Graphics.View EditorView { get; private set; }
        public GlyphWpfViewer Viewer { get; private set; }
        public IEnumerable<IView> RunnerViews => _runner.Engine.ViewManager.Views.Where(x => !(x.Camera.Parent is FreeCamera));
        GlyphEngine IDocumentContext<GlyphEngine>.Context => Runner?.Engine;

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    _runner.Engine.ViewManager.UnregisterView(EditorView);
                    _runner.Engine.Root.RemoveAndDispose(EditorCamera);
                }
                
                _runner = value;

                if (_runner != null)
                {
                    _shapedObjectSelector = _runner.Engine.Root.Add<ShapedObjectSelector>();
                    _shapedObjectSelector.Control = new HybridControl<System.Numerics.Vector2>("Pointer")
                    {
                        TriggerControl = new Control(InputSystem.Instance.Mouse[MouseButton.Left]),
                        ValueControl = new SceneCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, _runner.Engine.InputClientManager, _runner.Engine.ViewManager)
                    };
                    _shapedObjectSelector.HandleInputs = true;
                    _shapedObjectSelector.SelectionChanged += ShapedObjectSelectorOnSelectionChanged;

                    EditorView = _runner.Engine.Injector.Resolve<Glyph.Graphics.View>();
                    EditorView.Name = "Editor View";
                    EditorView.BoundingBox = new TopLeftRectangle(Vector2.Zero, VirtualResolution.Size);
                    EditorView.DrawClientFilter = new ExcludingFilter<IDrawClient>();

                    EditorCamera = _runner.Engine.Root.Add<FreeCamera>();
                    EditorCamera.View = EditorView;
                    _runner.Engine.ViewManager.RegisterView(EditorView);

                    _runner.Engine.Root.Schedulers.Update.Plan(EditorCamera).AtStart();
                }
                
                ConnectView();
                
                if (IsActive)
                    OnActivated();
            }
        }

        public Cursor ViewerCursor
        {
            get => _viewerCursor;
            set
            {
                if (_viewerCursor == value)
                    return;

                _viewerCursor = value;
                NotifyOfPropertyChange();
            }
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public ICommand FreeCameraCommand { get; }
        public ICommand DefaultViewsCommand { get; }

        public ICommand SelectViewCommand { get; }
        public ICommand NewViewerCommand { get; }

        public ICommand CursorInputsCommand { get; }
        public ICommand DefaultInputsCommand { get; }
        
        public SceneViewerViewModel(IShell shell, ContentManagerProvider contentManagerProvider, IEventAggregator eventAggregator)
        {
            _shell = shell;
            _contentManagerProvider = contentManagerProvider;
            _eventAggregator = eventAggregator;

            DisplayName = "Scene Viewer";

            PlayCommand = new RelayCommand(x => Runner.Engine.Start(), x => Runner?.Engine != null && !(Runner.Engine.IsStarted && !Runner.Engine.IsPaused));
            PauseCommand = new RelayCommand(x => Runner.Engine.Pause(), x => Runner?.Engine != null && Runner.Engine.IsStarted && !Runner.Engine.IsPaused);
            StopCommand = new RelayCommand(x => Runner.Engine.Stop(), x => Runner?.Engine?.IsStarted ?? false);

            FreeCameraCommand = new RelayCommand(FreeCameraAction, x => Runner?.Engine != null);
            DefaultViewsCommand = new RelayCommand(DefaultViewsAction, x => Runner?.Engine != null);

            SelectViewCommand = new RelayCommand(SelectViewAction, x => Runner?.Engine != null);
            NewViewerCommand = new RelayCommand(NewViewerAction, x => Runner?.Engine != null);

            CursorInputsCommand = new RelayCommand(CursorInputsAction, x => Runner?.Engine != null);
            DefaultInputsCommand = new RelayCommand(DefaultInputsAction, x => Runner?.Engine != null);
        }

        public void InitializeSession()
        {
            var engine = new GlyphEngine(_contentManagerProvider.Get(Session.ContentPath));
            Session.PrepareSession(engine);

            Runner = new GlyphWpfRunner
            {
                Engine = engine
            };
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (SceneViewerView)view;
            Viewer = _view.GlyphWpfViewer;

            ConnectView();

            Activated += OnActivated;
            Deactivated += OnDeactivated;
        }

        private void ConnectView()
        {
            if (_view == null)
                return;

            _view.ViewsComboBox.SelectedItems.Clear();

            if (Runner == null)
                return;
            
            Viewer.Runner = Runner;
            EditorCamera.Client = Viewer;
            Runner.Engine.FocusedClient = Viewer;

            foreach (IView runnerView in RunnerViews.Where(x => x.DrawClientFilter == null || x.DrawClientFilter.Filter(Viewer)))
                _view.ViewsComboBox.SelectedItems.Add(runnerView);

            FreeCameraAction(null);
        }

        private void ShapedObjectSelectorOnSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (_selectionRenderer != null)
            {
                Runner.Engine.Root.Remove(_selectionRenderer);
                _selectionRenderer.Dispose();
            }

            if (boxedComponent != null)
            {
                _selectionRenderer = new AreaComponentRenderer(boxedComponent, Runner.Engine.Injector.Resolve<Func<GraphicsDevice>>())
                {
                    Name = "Selection Renderer",
                    Color = Color.Purple * 0.5f,
                    DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera
                };
                Runner.Engine.Root.Add(_selectionRenderer);
            }

            _eventAggregator.PublishOnUIThread(boxedComponent != null ? new Selection<IGlyphComponent>(boxedComponent) : Selection<IGlyphComponent>.Empty);
        }

        private void FreeCameraAction(object obj)
        {
            foreach (IView runnerView in _runner.Engine.ViewManager.Views)
                SelectView(runnerView, runnerView == EditorView);
        }

        private void DefaultViewsAction(object obj)
        {
            foreach (IView runnerView in _runner.Engine.ViewManager.Views)
                SelectView(runnerView, _view.ViewsComboBox.SelectedItems.Contains(runnerView));
        }

        private void SelectViewAction(object obj)
        {
            if (EditorView.DrawClientFilter.Filter(Viewer))
                return;

            var runnerView = (IView)obj;
            SelectView(runnerView, _view.ViewsComboBox.SelectedItems.Contains(runnerView));
        }

        private void SelectView(IView view, bool isSelected)
        {
            if (view.DrawClientFilter == null)
                view.DrawClientFilter = new ExcludingFilter<IDrawClient>();

            if (isSelected ^ view.DrawClientFilter.Excluding)
                view.DrawClientFilter.Items.Add(Viewer);
            else
                view.DrawClientFilter.Items.Remove(Viewer);
        }

        private void NewViewerAction(object obj)
        {
            _shell.OpenDocument(new SceneViewerViewModel(_shell, _contentManagerProvider, _eventAggregator) { Runner = Runner });
        }

        private void CursorInputsAction(object obj)
        {
            foreach (ControlLayer controlLayer in Runner.Engine.ControlManager.Layers.Where(x => x.Tags.Contains(ControlLayerTag.Debug) || x.Tags.Contains(ControlLayerTag.Tools)))
                controlLayer.Enabled = true;

            ViewerCursor = Cursors.Cross;
        }

        private void DefaultInputsAction(object obj)
        {
            foreach (ControlLayer controlLayer in Runner.Engine.ControlManager.Layers.Where(x => x.Tags.Contains(ControlLayerTag.Debug) || x.Tags.Contains(ControlLayerTag.Tools)))
                controlLayer.Enabled = false;

            ViewerCursor = Cursors.None;
        }

        public void Dispose()
        {
            Activated -= OnActivated;
            Deactivated -= OnDeactivated;

            Runner?.Dispose();
            Runner = null;
        }

        private void OnActivated()
        {
            if (Runner?.Engine != null)
                Runner.Engine.FocusedClient = Viewer;
            
            _eventAggregator.PublishOnUIThread(new DocumentContext<GlyphEngine>(_runner?.Engine));
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => OnActivated();

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            if (Runner?.Engine != null && Runner.Engine.FocusedClient == Viewer)
                Runner.Engine.FocusedClient = null;
        }
    }
}