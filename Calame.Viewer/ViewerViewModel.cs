using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.ReadOnly;
using Fingear;
using Fingear.Interactives;
using Glyph;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Tools;
using Glyph.WpfInterop;
using MahApps.Metro.IconPacks;

namespace Calame.Viewer
{
    public class ViewerViewModel : PropertyChangedBase, IDisposable
    {
        private readonly IViewerViewModelOwner _owner;
        private readonly IEventAggregator _eventAggregator;
        
        private GlyphWpfRunner _runner;
        private IInteractive _editorInteractive;

        public IWpfGlyphClient Client { get; private set; }
        public FillView EditorView { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        public GlyphObject EditorRoot { get; private set; }

        public ReadOnlyCollection<IViewerModule> Modules { get; }
        public ObservableCollection<IViewerMode> InteractiveModules { get; }
        public InteractiveToggle InteractiveToggle { get; private set; }
        public SessionModeModule SessionMode { get; private set; }

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    GlyphEngine engine = _runner.Engine;

                    foreach (IViewerModule module in Modules)
                        module.Disconnect();
                    
                    engine.InteractionManager.Root.Remove(_editorInteractive);
                    engine.InteractionManager.Root.Remove(InteractiveToggle);
                    _runner.Engine.Root.RemoveAndDispose(EditorRoot);

                    _editorInteractive = null;
                    EditorView = null;
                    EditorCamera = null;
                    EditorRoot = null;

                    InteractiveToggle = null;
                    SessionMode = null;
                }

                _runner = value;

                if (_runner != null)
                {
                    GlyphEngine engine = _runner.Engine;

                    EditorRoot = engine.Root.Add<GlyphObject>();
                    EditorRoot.Name = "Editor Root";
                    EditorRoot.Add<SceneNode>().MakesRoot();

                    _editorInteractive = EditorRoot.Add<InteractiveRoot>().Interactive;
                    engine.InteractionManager.Root.Add(_editorInteractive);

                    SessionMode = new SessionModeModule();
                    InteractiveModules.Add(SessionMode);

                    InteractiveToggle = new InteractiveToggle
                    {
                        Components =
                        {
                            SessionMode.Interactive
                        },
                        SelectedInteractive = SessionMode.Interactive
                    };
                    engine.InteractionManager.Root.Add(InteractiveToggle);

                    EditorView = engine.Root.Add<FillView>();
                    EditorView.Name = "Editor View";
                    EditorView.ParentView = engine.RootView;
                    EditorView.DrawClientFilter = new ExcludingFilter<IDrawClient>();
                    
                    EditorCamera = EditorRoot.Add<FreeCamera>();
                    ConnectRunner();
                    EditorCamera.View = EditorView;

                    foreach (IViewerModule module in Modules)
                        module.Connect(this);
                }
                else
                    ConnectRunner();

                RunnerChanged?.Invoke(this, Runner);
                Activate();
            }
        }

        public event EventHandler<GlyphWpfRunner> RunnerChanged;

        public ViewerViewModel(IViewerViewModelOwner owner, IEventAggregator eventAggregator, IEnumerable<IViewerModule> modules)
        {
            _owner = owner;
            _eventAggregator = eventAggregator;
            Modules = new ReadOnlyCollection<IViewerModule>(modules.ToArray());
            InteractiveModules = new ObservableCollection<IViewerMode>();

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

        private void Activate()
        {
            if (Runner?.Engine != null)
                Runner.Engine.FocusedClient = Client;
            
            _eventAggregator.PublishOnUIThread(new DocumentContext<ViewerViewModel>(this));
            _eventAggregator.PublishOnUIThread(new DocumentContext<GlyphEngine>(_runner?.Engine));
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => Activate();

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            if (Runner?.Engine != null && Runner.Engine.FocusedClient == Client)
                Runner.Engine.FocusedClient = null;
        }

        public void Dispose()
        {
            _owner.Activated -= OnActivated;
            _owner.Deactivated -= OnDeactivated;

            foreach (IViewerModule module in Modules)
                module.Disconnect();

            _eventAggregator.Unsubscribe(this);

            Runner?.Dispose();
            Runner = null;
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