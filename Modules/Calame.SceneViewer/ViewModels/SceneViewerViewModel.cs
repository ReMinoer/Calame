using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Icons;
using Calame.Viewer;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Diese.Collections;
using Fingear.Interactives;
using Fingear.Interactives.Containers;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.WpfInterop;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : CalameDocumentBase, IViewerDocument, IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private readonly IShell _shell;
        private readonly IContentLibraryProvider _contentLibraryProvider;

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

        private readonly IIconDescriptorManager _iconDescriptorManager;
        public IIconProvider IconProvider { get; }
        public IIconDescriptor CalameIconDescriptor { get; }

        [ImportingConstructor]
        public SceneViewerViewModel(IEventAggregator eventAggregator, IShell shell, IContentLibraryProvider contentLibraryProvider,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
            : base(eventAggregator)
        {
            DisplayName = "Scene Viewer";

            _shell = shell;
            _contentLibraryProvider = contentLibraryProvider;

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModuleSources);
            Viewer.RunnerChanged += ViewerViewModelOnRunnerChanged;

            SessionMode = new SessionModeModule();
            Viewer.InsertInteractiveMode(0, SessionMode);

            PlayCommand = new RelayCommand(x => Runner.Engine.Start(), x => Runner?.Engine != null && !(Runner.Engine.IsStarted && !Runner.Engine.IsPaused));
            PauseCommand = new RelayCommand(x => Runner.Engine.Pause(), x => Runner?.Engine != null && Runner.Engine.IsStarted && !Runner.Engine.IsPaused);
            StopCommand = new RelayCommand(x => Runner.Engine.Stop(), x => Runner?.Engine?.IsStarted ?? false);

            FreeCameraCommand = new RelayCommand(x => FreeCameraAction(), x => Runner?.Engine != null);
            DefaultViewsCommand = new RelayCommand(x => DefaultViewsAction(), x => Runner?.Engine != null);
            NewViewerCommand = new RelayCommand(x => NewViewerAction(), x => Runner?.Engine != null);
            
            SwitchModeCommand = new RelayCommand(x => Viewer.SelectedMode = (IViewerInteractiveMode)x, x => Runner?.Engine != null);

            _iconDescriptorManager = iconDescriptorManager;
            IconProvider = iconProvider;
            CalameIconDescriptor = iconDescriptorManager.GetDescriptor<CalameIconKey>();
        }
        
        public SceneViewerViewModel(SceneViewerViewModel viewModel)
            : this(viewModel.EventAggregator, viewModel._shell, viewModel._contentLibraryProvider, viewModel.IconProvider, viewModel._iconDescriptorManager, Enumerable.Empty<IViewerModuleSource>())
        {
            throw new NotSupportedException();

            //_viewTracker = viewModel._viewTracker;
            //Session = viewModel.Session;

            //Viewer.Runner = viewModel.Runner;
        }

        public async Task InitializeSession()
        {
            _viewTracker?.Dispose();
            _viewTracker = null;

            _engine = new GlyphEngine(_contentLibraryProvider.Get(Session.ContentPath));
            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            var sessionView = _engine.Root.Add<FillView>();
            sessionView.ParentView = _engine.RootView;

            _viewTracker = _engine.Resolver.Resolve<MessagingTracker<IView>>();

            Viewer.Runner = new GlyphWpfRunner { Engine = _engine };

            var context = new SessionContext(Viewer, sessionView, SessionMode.Interactive);
            Session.PrepareSession(context);

            _engine.Initialize();
            _engine.LoadContent();

            FreeCameraAction();
            Viewer.EditorCamera.ShowTarget(context.UserRoot);

            _engine.Start();

            Viewer.SelectedMode = SessionMode;
            await Viewer.Activate();
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

        protected override Task DisposeDocumentAsync()
        {
            Viewer.RunnerChanged -= ViewerViewModelOnRunnerChanged;

            _engine.Stop();
            _viewTracker?.Dispose();

            Viewer.Dispose();

            return Task.CompletedTask;
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
            Task.Run(async () => await _shell.OpenDocumentAsync(new SceneViewerViewModel(this))).Wait();
        }

        async Task IHandle<ISelectionRequest<IGlyphComponent>>.HandleAsync(ISelectionRequest<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext != this)
                return;

            ISelectionSpread<IGlyphComponent> selection = message.Promoted;

            Viewer.LastSelection = selection;
            await EventAggregator.PublishAsync(selection, cancellationToken);
        }

        public class SessionModeModule : ViewerModuleBase, IViewerInteractiveMode
        {
            public string Name => "Session";
            public object IconKey => CalameIconKey.GameMode;

            public InteractiveComposite Interactive { get; } = new InteractiveComposite();
            IInteractive IViewerInteractiveMode.Interactive => Interactive;

            public Cursor Cursor => Cursors.None;
            public bool UseFreeCamera => false;

            protected override void ConnectRunner()
            {
                Model.AddInteractiveMode(this);
            }

            protected override void DisconnectRunner()
            {
                Model.RemoveInteractiveMode(this);
            }
        }
    }
}