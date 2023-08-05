using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.Viewer.Messages;
using Calame.Viewer.Modules;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Diese.Collections.ReadOnly;
using Fingear.Interactives;
using Fingear.Interactives.Containers;
using Fingear.Interactives.Interfaces;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Tools;
using Glyph.UI;
using Glyph.WpfInterop;

namespace Calame.Viewer.ViewModels
{
    public class ViewerViewModel : PropertyChangedBase, IHandle<ISwitchViewerModeRequest>, IDisposable
    {
        private readonly IViewerViewModelOwner _owner;
        private readonly IEventAggregator _eventAggregator;

        private GlyphWpfRunner _runner;
        private readonly InteractiveToggle _viewerModeToggle;
        private readonly ObservableList<IViewerInteractiveMode> _interactiveModes;
        private readonly EditorModeModule _editorModeModule;
        private IInteractive _editorInteractive;

        public IWpfGlyphClient Client { get; private set; }
        public FillView EditorView { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        
        public GlyphObject Root { get; private set; }
        public GlyphObject UserRoot { get; private set; }
        public GlyphObject EditorRoot { get; private set; }
        public GlyphObject EditorModeRoot => _editorModeModule.Root;

        public ReadOnlyList<IViewerModule> Modules { get; }
        public ReadOnlyObservableList<IViewerInteractiveMode> InteractiveModes { get; }
        
        public ObservableList<IGlyphComponent> NotSelectableComponents { get; }
        public ISelectionSpread<object> LastSelection { get; set; }

        private IViewerInteractiveMode _selectedMode;
        public IViewerInteractiveMode SelectedMode
        {
            get => _selectedMode;
            private set => this.SetValue(ref _selectedMode, value);
        }

        private Cursor _cursor;
        public Cursor Cursor
        {
            get => _cursor;
            private set => this.SetValue(ref _cursor, value);
        }

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    GlyphEngine engine = _runner.Engine;

                    _viewerModeToggle.Clear();

                    foreach (IViewerModule module in Modules.Reverse())
                        module.Disconnect();
                    
                    NotSelectableComponents.Clear();

                    engine?.InteractionManager.Root.Remove(_viewerModeToggle);
                    engine?.InteractionManager.Root.Remove(_editorInteractive);

                    engine?.Root.RemoveAndDispose(Root);
                    
                    EditorView = null;
                    EditorCamera = null;
                    EditorRoot = null;
                    UserRoot = null;
                    Root = null;
                }

                _runner = value;

                if (_runner != null)
                {
                    GlyphEngine engine = _runner.Engine;

                    Root = engine.Root.Add<GlyphObject>();
                    Root.Name = "Root";
                    Root.Add<SceneNode>().MakesRoot();
                    
                    UserRoot = Root.Add<GlyphObject>();
                    UserRoot.Name = "User Root";

                    EditorRoot = Root.Add<GlyphObject>();
                    EditorRoot.Name = "Editor Root";

                    _editorInteractive = EditorRoot.Add<InteractiveRoot>().Interactive;
                    engine.InteractionManager.Root.Add(_editorInteractive);
                    engine.InteractionManager.Root.Add(_viewerModeToggle);

                    EditorView = engine.Root.Add<FillView>();
                    EditorView.Name = "Editor View";
                    EditorView.ParentView = engine.RootView;

                    EditorCamera = EditorRoot.Add<FreeCamera>();
                    ConnectRunner();
                    EditorCamera.View = EditorView;

                    foreach (IViewerModule module in Modules)
                        module.Connect();

                    foreach (IViewerInteractiveMode interactiveMode in InteractiveModes)
                        _viewerModeToggle.Add(interactiveMode.Interactive);
                }
                else
                    ConnectRunner();

                RunnerChanged?.Invoke(this, Runner);
            }
        }

        public event EventHandler<GlyphWpfRunner> RunnerChanged;

        public ViewerViewModel(IViewerViewModelOwner owner, IEventAggregator eventAggregator, IEnumerable<IViewerModuleSource> moduleSources)
        {
            _owner = owner;
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUI(this);

            _viewerModeToggle = new InteractiveToggle { Name = "Viewer Modes" };

            _interactiveModes = new ObservableList<IViewerInteractiveMode>();
            InteractiveModes = new ReadOnlyObservableList<IViewerInteractiveMode>(_interactiveModes);

            _editorModeModule = new EditorModeModule();
            var componentSelectorModule = new BoxedComponentSelectorModule(_eventAggregator, (owner as IDocumentContext<ISelectionContext<IGlyphComponent>>)?.Context);

            var modules = new List<IViewerModule> { _editorModeModule, componentSelectorModule };
            modules.AddRange(moduleSources.Where(x => x.IsValidForDocument(owner)).Select(x => x.CreateInstance(owner)));

            foreach (IViewerModule module in modules)
                module.Model = this;

            Modules = new ReadOnlyList<IViewerModule>(modules);
            
            NotSelectableComponents = new ObservableList<IGlyphComponent>();

            _owner.Activated += OnActivated;
            _owner.Deactivated += OnDeactivated;
        }

        public void ConnectView(IViewerView view)
        {
            Client = view?.Client;
            ConnectRunner();
        }

        private void ConnectRunner()
        {
            if (Client == null || Runner == null)
                return;

            Client.Runner = Runner;
            EditorCamera.Client = Client;
            Runner.Engine.FocusedClient = Client;
        }

        public void AddInteractiveMode(IViewerInteractiveMode interactiveMode)
        {
            _interactiveModes.Add(interactiveMode);

            if (Runner != null)
                _viewerModeToggle.Add(interactiveMode.Interactive);
        }

        public void InsertInteractiveMode(int index, IViewerInteractiveMode interactiveMode)
        {
            _interactiveModes.Insert(index, interactiveMode);

            if (Runner != null)
                _viewerModeToggle.Add(interactiveMode.Interactive);
        }

        public void RemoveInteractiveMode(IViewerInteractiveMode interactiveMode)
        {
            if (Runner != null)
                _viewerModeToggle.Remove(interactiveMode.Interactive);

            _interactiveModes.Remove(interactiveMode);
        }

        public Task HandleAsync(ISwitchViewerModeRequest message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext != _owner)
                return Task.CompletedTask;

            IViewerInteractiveMode mode = InteractiveModes.FirstOrDefault(message.Match);
            if (mode == null)
                return Task.CompletedTask;

            SelectedMode?.OnUnselected();

            SelectedMode = mode;
            _viewerModeToggle.SelectedInteractive = SelectedMode.Interactive;

            SelectedMode?.OnSelected();

            Cursor = SelectedMode.Cursor;
            EditorCamera.Enabled = SelectedMode.UseFreeCamera;

            ISwitchViewerModeSpread messageSpread = message.Promoted(mode);
            return _eventAggregator.PublishAsync(messageSpread, cancellationToken);
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => ActivateAsync();

        public async Task ActivateAsync()
        {
            if (Runner?.Engine == null)
                return;

            Runner.Engine.FocusedClient = Client;

            foreach (IViewerModule module in Modules)
                module.Activate();

            if (LastSelection != null)
                await _eventAggregator.PublishAsync(LastSelection);
        }

        private Task OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            foreach (IViewerModule module in Modules)
                module.Deactivate();

            if (Runner?.Engine != null && Runner.Engine.FocusedClient == Client)
                Runner.Engine.FocusedClient = null;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (Client != null)
            {
                _owner.Activated -= OnActivated;
                _owner.Deactivated -= OnDeactivated;
            }

            foreach (IViewerModule module in Modules.Reverse())
                module.Disconnect();

            Runner?.Dispose();
            Runner = null;
        }

        public class EditorModeModule : ViewerModuleBase, IViewerInteractiveMode
        {
            private InterfaceRoot _interfaceRoot;

            public string Name => "Editor";
            public object IconKey => CalameIconKey.EditorMode;
            public Cursor Cursor => Cursors.Cross;
            public bool UseFreeCamera => true;
            bool IViewerInteractiveMode.IsUserMode => false;

            public GlyphObject Root { get; private set; }

            public InteractiveInterfaceRoot<IGlyphInteractiveInterface> Interactive => _interfaceRoot.Interactive;
            IInteractive IViewerInteractiveMode.Interactive => Interactive;

            protected override void ConnectViewer() => Model.AddInteractiveMode(this);
            protected override void DisconnectViewer() => Model.RemoveInteractiveMode(this);
            public override void Activate() { }
            public override void Deactivate() { }

            protected override void ConnectRunner()
            {
                _interfaceRoot = Model.EditorRoot.Add<InterfaceRoot>();
                _interfaceRoot.RaycastClient = Model.Client;

                Root = Model.EditorRoot.Add<GlyphObject>();
                Root.Name = "Editor Mode Root";
                Root.Add<UserInterface>();
            }

            protected override void DisconnectRunner()
            {
                Model.EditorRoot.RemoveAndDispose(Root);
                Root = null;

                Model.EditorRoot.RemoveAndDispose(_interfaceRoot);
                _interfaceRoot = null;
            }

            void IViewerInteractiveMode.OnSelected() {}
            void IViewerInteractiveMode.OnUnselected() {}
        }
    }
}