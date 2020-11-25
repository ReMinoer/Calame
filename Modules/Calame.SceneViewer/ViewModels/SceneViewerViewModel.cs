using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Icons;
using Calame.SceneViewer.Commands;
using Calame.Viewer;
using Calame.Viewer.Messages;
using Calame.Viewer.Modules.Base;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Diese.Collections;
using Fingear.Interactives;
using Fingear.Interactives.Containers;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.ToolBars;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : CalameDocumentBase, IViewerDocument, IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private readonly IShell _shell;

        private GlyphEngine _engine;
        private MessagingTracker<IView> _viewTracker;

        public ViewerViewModel Viewer { get; }
        public ISession Session { get; set; }
        public SessionModeModule SessionMode { get; private set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => Viewer.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => Viewer;
        IComponentFilter IDocumentContext<IComponentFilter>.Context => Viewer.ComponentsFilter;

        public ICommand FreeCameraCommand { get; }
        public ICommand DefaultViewsCommand { get; }
        public ICommand NewViewerCommand { get; }

        public ICommand SwitchModeCommand { get; }

        private GlyphWpfRunner Runner => Viewer.Runner;

        private readonly IIconDescriptorManager _iconDescriptorManager;
        public IIconProvider IconProvider { get; }
        public IIconDescriptor CalameIconDescriptor { get; }

        public string WorkingDirectory => _engine?.ContentLibrary?.WorkingDirectory;

        [ImportingConstructor]
        public SceneViewerViewModel(IEventAggregator eventAggregator, IShell shell,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
            : base(eventAggregator)
        {
            DisplayName = "Scene Viewer";

            _shell = shell;

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModuleSources);
            Viewer.RunnerChanged += ViewerViewModelOnRunnerChanged;

            SessionMode = new SessionModeModule();
            Viewer.InsertInteractiveMode(0, SessionMode);

            ToolBarDefinition = SceneToolBar;

            FreeCameraCommand = new RelayCommand(x => FreeCameraAction(), x => Runner?.Engine != null);
            DefaultViewsCommand = new RelayCommand(x => DefaultViewsAction(), x => Runner?.Engine != null);
            NewViewerCommand = new RelayCommand(x => NewViewerAction(), x => Runner?.Engine != null);
            
            SwitchModeCommand = new RelayCommand(OnSwitchMode, x => Runner?.Engine != null);

            _iconDescriptorManager = iconDescriptorManager;
            IconProvider = iconProvider;
            CalameIconDescriptor = iconDescriptorManager.GetDescriptor<CalameIconKey>();
        }

        [Export]
        static public ToolBarDefinition SceneToolBar = new ToolBarDefinition(0, "Scene");
        [Export]
        static public ToolBarItemGroupDefinition EngineToolBarGroup = new ToolBarItemGroupDefinition(SceneToolBar, 0);
        [Export]
        static public ToolBarItemDefinition EnginePauseResumeToolBarItem = new CommandToolBarItemDefinition<EnginePauseResumeCommand>(EngineToolBarGroup, 0);

        public SceneViewerViewModel(SceneViewerViewModel viewModel)
            : this(viewModel.EventAggregator, viewModel._shell, viewModel.IconProvider, viewModel._iconDescriptorManager, Enumerable.Empty<IViewerModuleSource>())
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

            IGraphicsDeviceService graphicsDeviceService = WpfGraphicsDeviceService.Instance;
            IContentLibrary contentLibrary = Session.CreateContentLibrary(graphicsDeviceService);

            _engine = new GlyphEngine(graphicsDeviceService, contentLibrary);

            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            var sessionView = _engine.Root.Add<FillView>();
            sessionView.ParentView = _engine.RootView;

            _viewTracker = _engine.Resolver.Resolve<MessagingTracker<IView>>();

            Viewer.Runner = new GlyphWpfRunner { Engine = _engine };

            var context = new SessionContext(Viewer, sessionView, SessionMode.Interactive);
            Session.PrepareSession(context);

            _engine.Initialize();
            await _engine.LoadContentAsync();

            FreeCameraAction();
            Viewer.EditorCamera.ShowTarget(context.UserRoot);
            Viewer.EditorCamera.SaveAsDefault();

            _engine.Start();
            await Viewer.Activate();

            await EventAggregator.PublishAsync(new SwitchViewerModeRequest(this, SessionMode));
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

        private async void OnSwitchMode(object obj)
        {
            await EventAggregator.PublishAsync(new SwitchViewerModeRequest(this, (IViewerInteractiveMode)obj));
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