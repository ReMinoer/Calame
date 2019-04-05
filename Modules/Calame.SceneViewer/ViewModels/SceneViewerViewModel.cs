using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Viewer;
using Calame.Viewer.Modules;
using Caliburn.Micro;
using Diese.Collections;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.WpfInterop;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : Document, IViewerViewModelOwner, IDocumentContext<GlyphEngine>, IDisposable
    {
        private readonly IShell _shell;
        private readonly IContentManagerProvider _contentManagerProvider;
        private readonly IEventAggregator _eventAggregator;
        
        private GlyphEngine _engine;
        private readonly ViewerViewModel _viewerViewModel;
        private Cursor _viewerCursor;
        private MessagingTracker<IView> _viewTracker;

        public ISession Session { get; set; }
        public GlyphWpfRunner Runner => _viewerViewModel.Runner;
        GlyphEngine IDocumentContext<GlyphEngine>.Context => _viewerViewModel.Runner?.Engine;

        public Cursor ViewerCursor
        {
            get => _viewerCursor;
            set => this.SetValue(ref _viewerCursor, value);
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public ICommand FreeCameraCommand { get; }
        public ICommand DefaultViewsCommand { get; }
        public ICommand NewViewerCommand { get; }

        public ICommand CursorInputsCommand { get; }
        public ICommand DefaultInputsCommand { get; }
        
        public SceneViewerViewModel(IShell shell, IContentManagerProvider contentManagerProvider, IEventAggregator eventAggregator)
        {
            _shell = shell;
            _contentManagerProvider = contentManagerProvider;
            _eventAggregator = eventAggregator;

            var viewerModules = new IViewerModule[]
            {
                new SceneNodeEditorModule(eventAggregator),
                new BoxedComponentSelectorModule(eventAggregator),
                new SelectionRendererModule(eventAggregator)
            };

            _viewerViewModel = new ViewerViewModel(this, _eventAggregator, viewerModules);
            _viewerViewModel.RunnerChanged += ViewerViewModelOnRunnerChanged;

            DisplayName = "Scene Viewer";

            PlayCommand = new RelayCommand(x => Runner.Engine.Start(), x => Runner?.Engine != null && !(Runner.Engine.IsStarted && !Runner.Engine.IsPaused));
            PauseCommand = new RelayCommand(x => Runner.Engine.Pause(), x => Runner?.Engine != null && Runner.Engine.IsStarted && !Runner.Engine.IsPaused);
            StopCommand = new RelayCommand(x => Runner.Engine.Stop(), x => Runner?.Engine?.IsStarted ?? false);

            FreeCameraCommand = new RelayCommand(FreeCameraAction, x => Runner?.Engine != null);
            DefaultViewsCommand = new RelayCommand(DefaultViewsAction, x => Runner?.Engine != null);
            NewViewerCommand = new RelayCommand(NewViewerAction, x => Runner?.Engine != null);

            CursorInputsCommand = new RelayCommand(CursorInputsAction, x => Runner?.Engine != null);
            DefaultInputsCommand = new RelayCommand(DefaultInputsAction, x => Runner?.Engine != null);
        }
        
        public SceneViewerViewModel(SceneViewerViewModel viewModel)
            : this(viewModel._shell, viewModel._contentManagerProvider, viewModel._eventAggregator)
        {
            _viewTracker = viewModel._viewTracker;
            Session = viewModel.Session;

            _viewerViewModel.Runner = viewModel.Runner;
        }

        public void InitializeSession()
        {
            _viewTracker?.Dispose();
            _viewTracker = null;

            _engine = new GlyphEngine(_contentManagerProvider.Get(Session.ContentPath));
            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            var sessionView = _engine.Root.Add<FillView>();
            sessionView.ParentView = _engine.RootView;

            _viewTracker = _engine.Injector.Resolve<MessagingTracker<IView>>();

            _viewerViewModel.Runner = new GlyphWpfRunner { Engine = _engine };

            var context = new SessionContext(_viewerViewModel, sessionView);
            Session.PrepareSession(context);

            FreeCameraAction();

            _engine.Initialize();
            _engine.LoadContent();
            _engine.Start();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view == null)
                return;
            
            _viewerViewModel.ConnectView((IViewerView)view);
            FreeCameraAction();
        }

        private void FreeCameraAction(object obj = null)
        {
            if (_viewTracker == null)
                return;

            foreach (IView runnerView in _viewTracker)
                SelectView(runnerView, runnerView == _viewerViewModel.EditorView);
        }

        private void DefaultViewsAction(object obj = null)
        {
            if (_viewTracker == null)
                return;

            foreach (IView runnerView in _viewTracker)
                SelectView(runnerView, runnerView != _viewerViewModel.EditorView);
        }

        private void SelectView(IView view, bool isSelected)
        {
            if (view.DrawClientFilter == null)
                view.DrawClientFilter = new ExcludingFilter<IDrawClient>();

            if (isSelected ^ view.DrawClientFilter.Excluding)
                view.DrawClientFilter.Items.Add(_viewerViewModel.Client);
            else
                view.DrawClientFilter.Items.Remove(_viewerViewModel.Client);
        }

        private void NewViewerAction(object obj)
        {
            _shell.OpenDocument(new SceneViewerViewModel(this));
        }

        private void CursorInputsAction(object obj)
        {
            _viewerViewModel.EditorSessionInteractive.EditionMode = true;
            ViewerCursor = Cursors.Cross;
        }

        private void DefaultInputsAction(object obj)
        {
            _viewerViewModel.EditorSessionInteractive.EditionMode = false;
            ViewerCursor = Cursors.None;
        }

        private void ViewerViewModelOnRunnerChanged(object sender, GlyphWpfRunner e)
        {
            FreeCameraAction();
        }

        public void Dispose()
        {
            _engine.Stop();
            _viewTracker?.Dispose();

            _viewerViewModel.RunnerChanged -= ViewerViewModelOnRunnerChanged;
            _viewerViewModel.Dispose();
        }
    }
}