using System;
using Caliburn.Micro;
using Diese.Collections;
using Glyph;
using Glyph.Core;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Tools;
using Glyph.WpfInterop;

namespace Calame.Viewer
{
    public class ViewerViewModel : PropertyChangedBase, IDisposable
    {
        private readonly IViewerViewModelOwner _owner;
        private readonly IEventAggregator _eventAggregator;
        
        private GlyphWpfRunner _runner;
        private readonly IViewerModule[] _modules;

        public IWpfGlyphClient Client { get; private set; }
        public FillView EditorView { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        public GlyphObject EditorRoot { get; private set; }

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    foreach (IViewerModule module in _modules)
                        module.Disconnect();

                    _runner.Engine.Root.RemoveAndDispose(EditorRoot);
                    EditorRoot = null;
                }

                _runner = value;

                if (_runner != null)
                {
                    GlyphEngine engine = _runner.Engine;

                    EditorRoot = engine.Root.Add<GlyphObject>();
                    EditorRoot.Name = "Editor Root";
                    EditorRoot.Add<SceneNode>().MakesRoot();

                    EditorView = engine.Root.Add<FillView>();
                    EditorView.Name = "Editor View";
                    EditorView.ParentView = engine.RootView;
                    EditorView.DrawClientFilter = new ExcludingFilter<IDrawClient>();
                    
                    EditorCamera = EditorRoot.Add<FreeCamera>();
                    ConnectRunner();
                    EditorCamera.View = EditorView;

                    foreach (IViewerModule module in _modules)
                        module.Connect(this);
                }
                else
                    ConnectRunner();

                RunnerChanged?.Invoke(this, Runner);
                Activate();
            }
        }

        public event EventHandler<GlyphWpfRunner> RunnerChanged;

        public ViewerViewModel(IViewerViewModelOwner owner, IEventAggregator eventAggregator, IViewerModule[] modules)
        {
            _owner = owner;
            _eventAggregator = eventAggregator;
            _modules = modules;

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

            foreach (IViewerModule module in _modules)
                module.Disconnect();

            _eventAggregator.Unsubscribe(this);

            Runner?.Dispose();
            Runner = null;
        }
    }
}