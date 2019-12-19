using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Calame.Viewer;
using Caliburn.Micro;
using Diese.Collections;
using Fingear;
using Fingear.Interactives;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.WpfInterop;
using MahApps.Metro.IconPacks;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : Document, IViewerViewModelOwner, IDocumentContext<GlyphEngine>, IDocumentContext<ViewerViewModel>, IHandle<ISelectionRequest<IGlyphComponent>>, IDisposable
    {
        private readonly IShell _shell;
        private readonly IContentLibraryProvider _contentLibraryProvider;
        private readonly IEventAggregator _eventAggregator;
        
        private GlyphEngine _engine;
        private Cursor _viewerCursor;
        private MessagingTracker<IView> _viewTracker;
        private IViewerMode _selectedMode;

        public ViewerViewModel Viewer { get; }
        public ISession Session { get; set; }
        public GlyphWpfRunner Runner => Viewer.Runner;
        public SessionModeModule SessionMode { get; private set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => Viewer.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => Viewer;

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

        public ICommand SwitchModeCommand { get; }
        
        [ImportingConstructor]
        public SceneViewerViewModel(IShell shell, IContentLibraryProvider contentLibraryProvider, IEventAggregator eventAggregator, [ImportMany] IEnumerable<IViewerModule> viewerModules)
        {
            _shell = shell;
            _contentLibraryProvider = contentLibraryProvider;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Viewer = new ViewerViewModel(this, _eventAggregator, viewerModules);
            Viewer.RunnerChanged += ViewerViewModelOnRunnerChanged;

            DisplayName = "Scene Viewer";

            PlayCommand = new RelayCommand(x => Runner.Engine.Start(), x => Runner?.Engine != null && !(Runner.Engine.IsStarted && !Runner.Engine.IsPaused));
            PauseCommand = new RelayCommand(x => Runner.Engine.Pause(), x => Runner?.Engine != null && Runner.Engine.IsStarted && !Runner.Engine.IsPaused);
            StopCommand = new RelayCommand(x => Runner.Engine.Stop(), x => Runner?.Engine?.IsStarted ?? false);

            FreeCameraCommand = new RelayCommand(x => FreeCameraAction(), x => Runner?.Engine != null);
            DefaultViewsCommand = new RelayCommand(x => DefaultViewsAction(), x => Runner?.Engine != null);
            NewViewerCommand = new RelayCommand(x => NewViewerAction(), x => Runner?.Engine != null);
            
            SwitchModeCommand = new RelayCommand(x => SwitchModeAction((IViewerMode)x), x => Runner?.Engine != null);
        }
        
        public SceneViewerViewModel(SceneViewerViewModel viewModel)
            : this(viewModel._shell, viewModel._contentLibraryProvider, viewModel._eventAggregator, Enumerable.Empty<IViewerModule>())
        {
            throw new NotSupportedException();

            //_viewTracker = viewModel._viewTracker;
            //Session = viewModel.Session;

            //Viewer.Runner = viewModel.Runner;
        }

        public void InitializeSession()
        {
            _viewTracker?.Dispose();
            _viewTracker = null;
            SessionMode = null;

            _engine = new GlyphEngine(_contentLibraryProvider.Get(Session.ContentPath));
            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            var sessionView = _engine.Root.Add<FillView>();
            sessionView.ParentView = _engine.RootView;

            _viewTracker = _engine.Resolver.Resolve<MessagingTracker<IView>>();

            Viewer.Runner = new GlyphWpfRunner { Engine = _engine };

            SessionMode = new SessionModeModule();
            Viewer.InteractiveToggle.Add(SessionMode.Interactive);
            Viewer.InteractiveModules.Add(SessionMode);

            var context = new SessionContext(Viewer, sessionView, SessionMode.Interactive);
            Session.PrepareSession(context);

            FreeCameraAction();

            _engine.Initialize();
            _engine.LoadContent();
            _engine.Start();

            SwitchModeAction(SessionMode);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view == null)
                return;
            
            Viewer.ConnectView((IViewerView)view);
            FreeCameraAction();
        }

        private void OnActivated()
        {
            _eventAggregator.PublishOnUIThread(this);
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => OnActivated();

        private void ViewerViewModelOnRunnerChanged(object sender, GlyphWpfRunner e)
        {
            FreeCameraAction();
            OnActivated();
            Activated += OnActivated;
        }

        private void FreeCameraAction()
        {
            if (_viewTracker == null)
                return;

            foreach (IView runnerView in _viewTracker)
                SelectView(runnerView, runnerView == Viewer.EditorView);
        }

        private void DefaultViewsAction()
        {
            if (_viewTracker == null)
                return;

            foreach (IView runnerView in _viewTracker)
                SelectView(runnerView, runnerView != Viewer.EditorView);
        }

        private void SelectView(IView view, bool isSelected)
        {
            if (view.DrawClientFilter == null)
                view.DrawClientFilter = new ExcludingFilter<IDrawClient>();

            if (isSelected ^ view.DrawClientFilter.Excluding)
                view.DrawClientFilter.Items.Add(Viewer.Client);
            else
                view.DrawClientFilter.Items.Remove(Viewer.Client);
        }

        private void NewViewerAction()
        {
            _shell.OpenDocument(new SceneViewerViewModel(this));
        }

        private void SwitchModeAction(IViewerMode mode)
        {
            if (_selectedMode == mode)
                return;
            
            _selectedMode = mode;

            Viewer.InteractiveToggle.SelectedInteractive = mode.Interactive;
            ViewerCursor = mode.Cursor;
            Viewer.EditorCamera.Enabled = mode.UseFreeCamera;
        }

        public void Handle(ISelectionRequest<IGlyphComponent> message)
        {
            if (message.DocumentContext != this)
                return;

            _eventAggregator.PublishOnUIThread(message.Promoted);
        }

        public void Dispose()
        {
            _engine.Stop();
            _viewTracker?.Dispose();

            Viewer.RunnerChanged -= ViewerViewModelOnRunnerChanged;
            Viewer.Dispose();
        }

        public class SessionModeModule : IViewerMode
        {
            public string Name => "Session";
            public object IconId => PackIconMaterialKind.GamepadVariant;

            public InteractiveComposite Interactive { get; } = new InteractiveComposite();
            IInteractive IViewerMode.Interactive => Interactive;

            public Cursor Cursor => Cursors.None;
            public bool UseFreeCamera => false;
        }
    }
}