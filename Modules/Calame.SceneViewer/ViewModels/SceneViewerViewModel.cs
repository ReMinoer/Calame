using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Icons;
using Calame.Viewer;
using Calame.Viewer.Messages;
using Calame.Viewer.Modules.Base;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Diese.Collections;
using Fingear.Interactives;
using Fingear.Interactives.Containers;
using Gemini.Framework;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.WpfInterop;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : CalameDocumentBase, IViewerDocument, IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private GlyphEngine _engine;
        private MessagingTracker<IView> _viewTracker;
        private ISession _session;

        public ISession Session
        {
            get => _session;
            set
            {
                _session = value;
                RefreshIcon();
            }
        }

        public ViewerViewModel Viewer { get; }
        public SessionModeModule SessionMode { get; }

        IDocument IDocumentContext.Document => this;
        GlyphEngine IDocumentContext<GlyphEngine>.Context => Viewer.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => Viewer;
        IComponentFilter IDocumentContext<IComponentFilter>.Context => Viewer.ComponentsFilter;

        public bool FreeCameraEnabled { get; private set; }

        public string WorkingDirectory => _engine?.ContentLibrary?.WorkingDirectory;

        [ImportingConstructor]
        public SceneViewerViewModel(IEventAggregator eventAggregator, ILoggerProvider loggerProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager,
            [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
            : base(eventAggregator, loggerProvider, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Scene Viewer";

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModuleSources);
            Viewer.RunnerChanged += ViewerViewModelOnRunnerChanged;

            SessionMode = new SessionModeModule();
            Viewer.InsertInteractiveMode(0, SessionMode);
        }

        public async Task InitializeSession()
        {
            _viewTracker?.Dispose();
            _viewTracker = null;

            IGraphicsDeviceService graphicsDeviceService = WpfGraphicsDeviceService.Instance;
            IContentLibrary contentLibrary = Session.CreateContentLibrary(graphicsDeviceService, Logger);

            _engine = new GlyphEngine(graphicsDeviceService, contentLibrary, Logger);

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

            EnableFreeCamera();
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
            EnableFreeCamera();
        }

        private void ViewerViewModelOnRunnerChanged(object sender, GlyphWpfRunner e)
        {
            EnableFreeCamera();
        }

        protected override Task DisposeDocumentAsync()
        {
            Viewer.RunnerChanged -= ViewerViewModelOnRunnerChanged;

            _engine.Stop();
            _viewTracker?.Dispose();

            Viewer.Dispose();

            return Task.CompletedTask;
        }

        public void EnableFreeCamera()
        {
            FreeCameraEnabled = true;

            if (_viewTracker == null)
                return;

            foreach (IView runnerView in _viewTracker)
                SelectView(runnerView, runnerView == Viewer.EditorView);
        }

        public void EnableDefaultCamera()
        {
            FreeCameraEnabled = false;

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
            public object IconKey => CalameIconKey.SessionMode;

            public InteractiveComposite Interactive { get; } = new InteractiveComposite();
            IInteractive IViewerInteractiveMode.Interactive => Interactive;

            public Cursor Cursor => Cursors.None;
            public bool UseFreeCamera => false;

            protected override void ConnectModel() => Model.AddInteractiveMode(this);
            protected override void DisconnectModel() => Model.RemoveInteractiveMode(this);
            protected override void ConnectRunner() {}
            protected override void DisconnectRunner() {}
        }
    }
}