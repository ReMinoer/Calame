using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Calame.SceneViewer.Views;
using Caliburn.Micro;
using Diese.Collections;
using Fingear;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Math.Shapes;
using Glyph.Tools;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : Document, IDisposable
    {
        private readonly IShell _shell;
        private readonly ContentManagerProvider _contentManagerProvider;
        
        private SceneViewerView _view;
        private Cursor _viewerCursor;
        private GlyphWpfRunner _runner;
        public ISession Session { get; set; }
        public FreeCamera EditorCamera { get; private set; }
        public Glyph.Graphics.View EditorView { get; private set; }
        public GlyphWpfViewer Viewer { get; private set; }
        public IEnumerable<IView> RunnerViews => ViewManager.Main.Views.Where(x => !(x.Camera.Parent is FreeCamera));

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    ViewManager.Main.UnregisterView(EditorView);
                    _runner.Engine.Root.RemoveAndDispose(EditorCamera);
                }
                
                _runner = value;

                if (_runner != null)
                {
                    EditorCamera = _runner.Engine.Root.Add<FreeCamera>();
                    EditorView = _runner.Engine.Injector.Resolve<Glyph.Graphics.View>();
                    EditorView.Name = "Editor View";
                    EditorView.BoundingBox = new TopLeftRectangle(Vector2.Zero, VirtualResolution.Size);
                    EditorView.DrawClientFilter = new ExcludingFilter<IDrawClient>();

                    EditorCamera.View = EditorView;
                    ViewManager.Main.RegisterView(EditorView);

                    _runner.Engine.Root.Schedulers.Update.Plan(EditorCamera).AtStart();
                }
                
                ConnectView();
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
        
        public SceneViewerViewModel(IShell shell, ContentManagerProvider contentManagerProvider)
        {
            _shell = shell;
            _contentManagerProvider = contentManagerProvider;

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

        private void FreeCameraAction(object obj)
        {
            foreach (IView runnerView in ViewManager.Main.Views)
                SelectView(runnerView, runnerView == EditorView);
        }

        private void DefaultViewsAction(object obj)
        {
            foreach (IView runnerView in ViewManager.Main.Views)
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
            _shell.OpenDocument(new SceneViewerViewModel(_shell, _contentManagerProvider) { Runner = Runner });
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
            
            Runner?.Dispose();
            Runner = null;
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs)
        {
            if (Runner?.Engine != null)
                Runner.Engine.FocusedClient = Viewer;
        }
    }
}