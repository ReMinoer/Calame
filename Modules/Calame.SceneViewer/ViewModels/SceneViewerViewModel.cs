using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.SceneViewer.Commands;
using Calame.Viewer;
using Calame.Viewer.Messages;
using Calame.Viewer.Modules.Base;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Diese.Collections;
using Fingear.Inputs;
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
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SceneViewerViewModel : CalameDocumentBase, IViewerDocument, IRunnableDocument, IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private readonly IShell _shell;
        private readonly SelectionHistoryManager _selectionHistoryManager;

        private GlyphEngine _engine;
        private MessagingTracker<IView> _viewTracker;
        private ISession _session;
        private SessionContext _sessionContext;

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

        public bool FreeCameraEnabled { get; private set; }
        public Type RunCommandDefinitionType { get; } = typeof(ResetSessionCommand);

        private IGlyphComponent DefaultSelection => Viewer.UserRoot.Components.FirstOrDefault() ?? Viewer.UserRoot;

        [ImportingConstructor]
        public SceneViewerViewModel(IShell shell, IEventAggregator eventAggregator,
            ILoggerProvider loggerProvider, SelectionHistoryManager selectionHistoryManager,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager,
            [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
            : base(eventAggregator, loggerProvider, iconProvider, iconDescriptorManager)
        {
            _shell = shell;
            _selectionHistoryManager = selectionHistoryManager;

            DisplayName = "Scene Viewer";

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModuleSources);
            Viewer.RunnerChanged += ViewerViewModelOnRunnerChanged;

            _debuggableViewerContexts = new DebuggableViewerContexts(Viewer);

            SessionMode = new SessionModeModule();
            Viewer.InsertInteractiveMode(0, SessionMode);

            Fingear.Inputs.InputManager.Instance.InputSourcesUsed += OnInputSourcesUsed;
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

            _sessionContext = new SessionContext(Viewer, sessionView, SessionMode.Interactive);
            await Session.PrepareSessionAsync(_sessionContext);
            await Session.ResetSessionAsync(_sessionContext);

            _debuggableViewerContexts.RefreshDebuggableContexts();

            _engine.Initialize();
            await _engine.LoadContentAsync();

            EnableFreeCamera();
            Viewer.EditorCamera.ShowTarget(_sessionContext.UserRoot);

            _engine.Start();
            await Viewer.Activate();
            
            await EventAggregator.PublishAsync(new SelectionRequest<IGlyphComponent>(this, DefaultSelection));
            await EventAggregator.PublishAsync(new SwitchViewerModeRequest(this, SessionMode));

            CommandManager.InvalidateRequerySuggested();
        }

        public async Task ResetSessionAsync()
        {
            await EventAggregator.PublishAsync(SelectionRequest<IGlyphComponent>.Empty(this));

            Viewer.NotSelectableComponents.Clear();
            _selectionHistoryManager.GetHistory(this).Clear();

            await Session.ResetSessionAsync(_sessionContext);

            if (FreeCameraEnabled)
                EnableFreeCamera();
            else
                EnableDefaultCamera();
            
            await EventAggregator.PublishAsync(new SelectionRequest<IGlyphComponent>(this, DefaultSelection));
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
            Fingear.Inputs.InputManager.Instance.InputSourcesUsed -= OnInputSourcesUsed;
            Viewer.RunnerChanged -= ViewerViewModelOnRunnerChanged;

            _engine.Stop();

            _viewTracker?.Dispose();
            _viewTracker = null;

            Viewer.Dispose();

            _selectionHistoryManager.RemoveHistory(this);

            return Task.CompletedTask;
        }

        private void OnInputSourcesUsed(IReadOnlyCollection<IInputSource> usedInputSources)
        {
            if (_shell.ActiveItem != this)
                return;
            if (Viewer.SelectedMode == SessionMode)
                return;

            if (usedInputSources.Any(x => x.Type == InputSourceType.GamePad))
            {
                EventAggregator.PublishAsync(new SwitchViewerModeRequest(this, SessionMode)).Wait();
                CommandManager.InvalidateRequerySuggested();
            }
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
            bool IViewerInteractiveMode.IsUserMode => true;

            protected override void ConnectModel() => Model.AddInteractiveMode(this);
            protected override void DisconnectModel() => Model.RemoveInteractiveMode(this);
            protected override void ConnectRunner() {}
            protected override void DisconnectRunner() {}

            void IViewerInteractiveMode.OnSelected() {}
            void IViewerInteractiveMode.OnUnselected() {}
        }

        private readonly DebuggableViewerContexts _debuggableViewerContexts;

        bool IViewerDocument.DebugMode
        {
            get => _debuggableViewerContexts.DebugMode;
            set => _debuggableViewerContexts.DebugMode = value;
        }

        IDocument IDocumentContext.Document => this;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => _debuggableViewerContexts.Viewer;
        IContentLibraryContext IDocumentContext<IContentLibraryContext>.Context => _debuggableViewerContexts;
        IRawContentLibraryContext IDocumentContext<IRawContentLibraryContext>.Context => _debuggableViewerContexts;
        IRootsContext IDocumentContext<IRootsContext>.Context => _debuggableViewerContexts;
        IRootComponentsContext IDocumentContext<IRootComponentsContext>.Context => _debuggableViewerContexts;
        IRootScenesContext IDocumentContext<IRootScenesContext>.Context => _debuggableViewerContexts;
        IRootInteractivesContext IDocumentContext<IRootInteractivesContext>.Context => _debuggableViewerContexts;
        ISelectionContext IDocumentContext<ISelectionContext>.Context => this;
        ISelectionContext<IGlyphComponent> IDocumentContext<ISelectionContext<IGlyphComponent>>.Context => this;

        event EventHandler ISelectionContext.CanSelectChanged
        {
            add => _debuggableViewerContexts.CanSelectChanged += value;
            remove => _debuggableViewerContexts.CanSelectChanged -= value;
        }

        public bool CanSelect(object instance) => instance is IGlyphComponent component && CanSelect(component);
        public bool CanSelect(IGlyphComponent component) => _debuggableViewerContexts.CanSelect(component);

        public Task SelectAsync(object instance)
        {
            if (instance is IGlyphComponent component)
                return SelectAsync(component);
            return Task.CompletedTask;
        }

        public Task SelectAsync(IGlyphComponent component)
        {
            if (CanSelect(component))
                return EventAggregator.PublishAsync(new SelectionRequest<IGlyphComponent>(this, component));
            return Task.CompletedTask;
        }
    }
}