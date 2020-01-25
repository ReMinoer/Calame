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
    public sealed class SceneViewerViewModel : Document, IViewerDocument, IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private readonly IShell _shell;
        private readonly IContentLibraryProvider _contentLibraryProvider;
        private readonly IEventAggregator _eventAggregator;
        
        private GlyphEngine _engine;
        private MessagingTracker<IView> _viewTracker;

        public ViewerViewModel Viewer { get; }
        public ISession Session { get; set; }
        public SessionModeModule SessionMode { get; private set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => Viewer.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => Viewer;
        IComponentFilter IDocumentContext<IComponentFilter>.Context => Viewer.ComponentsFilter;

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public ICommand FreeCameraCommand { get; }
        public ICommand DefaultViewsCommand { get; }
        public ICommand NewViewerCommand { get; }

        public ICommand SwitchModeCommand { get; }

        private GlyphWpfRunner Runner => Viewer.Runner;
        
        [ImportingConstructor]
        public SceneViewerViewModel(IShell shell, IContentLibraryProvider contentLibraryProvider, IEventAggregator eventAggregator, [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
        {
            _shell = shell;
            _contentLibraryProvider = contentLibraryProvider;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Viewer = new ViewerViewModel(this, _eventAggregator, viewerModuleSources);
            Viewer.RunnerChanged += ViewerViewModelOnRunnerChanged;

            DisplayName = "Scene Viewer";

            PlayCommand = new RelayCommand(x => Runner.Engine.Start(), x => Runner?.Engine != null && !(Runner.Engine.IsStarted && !Runner.Engine.IsPaused));
            PauseCommand = new RelayCommand(x => Runner.Engine.Pause(), x => Runner?.Engine != null && Runner.Engine.IsStarted && !Runner.Engine.IsPaused);
            StopCommand = new RelayCommand(x => Runner.Engine.Stop(), x => Runner?.Engine?.IsStarted ?? false);

            FreeCameraCommand = new RelayCommand(x => FreeCameraAction(), x => Runner?.Engine != null);
            DefaultViewsCommand = new RelayCommand(x => DefaultViewsAction(), x => Runner?.Engine != null);
            NewViewerCommand = new RelayCommand(x => NewViewerAction(), x => Runner?.Engine != null);
            
            SwitchModeCommand = new RelayCommand(x => Viewer.SelectedMode = (IViewerInteractiveMode)x, x => Runner?.Engine != null);
        }
        
        public SceneViewerViewModel(SceneViewerViewModel viewModel)
            : this(viewModel._shell, viewModel._contentLibraryProvider, viewModel._eventAggregator, Enumerable.Empty<IViewerModuleSource>())
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
            Viewer.AddInteractiveMode(SessionMode);

            var context = new SessionContext(Viewer, sessionView, SessionMode.Interactive);
            Session.PrepareSession(context);

            FreeCameraAction();

            _engine.Initialize();
            _engine.LoadContent();
            _engine.Start();

            Viewer.SelectedMode = SessionMode;
            Viewer.Activate();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view == null)
                return;
            
            Viewer.ConnectView((IViewerView)view);
            FreeCameraAction();
        }

        private void ViewerViewModelOnRunnerChanged(object sender, GlyphWpfRunner e)
        {
            FreeCameraAction();
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

        public void Handle(ISelectionRequest<IGlyphComponent> message)
        {
            if (message.DocumentContext != this)
                return;

            ISelectionSpread<IGlyphComponent> selection = message.Promoted;

            Viewer.LastSelection = selection;
            _eventAggregator.PublishOnUIThread(selection);
        }

        public void Dispose()
        {
            Viewer.RunnerChanged -= ViewerViewModelOnRunnerChanged;

            _engine.Stop();
            _viewTracker?.Dispose();

            Viewer.Dispose();
        }

        public class SessionModeModule : IViewerInteractiveMode
        {
            public string Name => "Session";
            public object IconId => PackIconMaterialKind.GamepadVariant;

            public InteractiveComposite Interactive { get; } = new InteractiveComposite();
            IInteractive IViewerInteractiveMode.Interactive => Interactive;

            public Cursor Cursor => Cursors.None;
            public bool UseFreeCamera => false;
        }
    }
}