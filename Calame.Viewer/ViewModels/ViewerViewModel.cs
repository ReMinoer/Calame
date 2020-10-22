using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private readonly ObservableList<ViewerInteractiveModeViewModel> _interactiveModes;
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
        public ReadOnlyObservableList<ViewerInteractiveModeViewModel> InteractiveModes { get; }

        public ComponentFilter ComponentsFilter { get; }
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

                    foreach (IViewerModule module in Modules)
                        module.Disconnect();

                    ComponentsFilter.ExcludedRoots.Clear();
                    
                    engine.InteractionManager.Root.Remove(_viewerModeToggle);
                    engine.InteractionManager.Root.Remove(_editorInteractive);
                    _runner.Engine.Root.RemoveAndDispose(Root);
                    
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
            }
        }

        public event EventHandler<GlyphWpfRunner> RunnerChanged;

        public ViewerViewModel(IViewerViewModelOwner owner, IEventAggregator eventAggregator, IEnumerable<IViewerModuleSource> moduleSources)
        {
            _owner = owner;
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUI(this);

            _viewerModeToggle = new InteractiveToggle { Name = "Viewer Modes" };

            _interactiveModes = new ObservableList<ViewerInteractiveModeViewModel>();
            InteractiveModes = new ReadOnlyObservableList<ViewerInteractiveModeViewModel>(_interactiveModes);

            _editorModeModule = new EditorModeModule();
            var componentSelectorModule = new BoxedComponentSelectorModule(_eventAggregator);

            var modules = new List<IViewerModule> { _editorModeModule, componentSelectorModule };
            modules.AddRange(moduleSources.Where(x => x.IsValidForDocument(owner)).Select(x => x.CreateInstance()));
            Modules = new ReadOnlyList<IViewerModule>(modules);

            ComponentsFilter = new ComponentFilter();

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
            _interactiveModes.Add(new ViewerInteractiveModeViewModel(interactiveMode));
            _viewerModeToggle.Add(interactiveMode.Interactive);
        }

        public void InsertInteractiveMode(int index, IViewerInteractiveMode interactiveMode)
        {
            _interactiveModes.Insert(index, new ViewerInteractiveModeViewModel(interactiveMode));
            _viewerModeToggle.Add(interactiveMode.Interactive);
        }

        public void RemoveInteractiveMode(IViewerInteractiveMode interactiveMode)
        {
            _viewerModeToggle.Remove(interactiveMode.Interactive);
            _interactiveModes.Remove(_interactiveModes.First(x => x.InteractiveModel == interactiveMode));
        }

        public Task HandleAsync(ISwitchViewerModeRequest message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext != _owner)
                return Task.CompletedTask;

            ViewerInteractiveModeViewModel modeViewModel = InteractiveModes.FirstOrDefault(x => message.Match(x.InteractiveModel));
            if (modeViewModel == null)
                return Task.CompletedTask;

            IViewerInteractiveMode mode = modeViewModel.InteractiveModel;

            SelectedMode = mode;
            modeViewModel.IsActive = true;
            _viewerModeToggle.SelectedInteractive = SelectedMode.Interactive;

            Cursor = SelectedMode.Cursor;
            EditorCamera.Enabled = SelectedMode.UseFreeCamera;

            ISwitchViewerModeSpread messageSpread = message.Promoted(mode);
            return _eventAggregator.PublishAsync(messageSpread, cancellationToken);
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => Activate();

        public async Task Activate()
        {
            if (Runner?.Engine == null)
                return;

            Runner.Engine.FocusedClient = Client;
            await _eventAggregator.PublishAsync(_owner);

            if (LastSelection != null)
                await _eventAggregator.PublishAsync(LastSelection);
        }

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            if (Runner?.Engine != null && Runner.Engine.FocusedClient == Client)
                Runner.Engine.FocusedClient = null;
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
        }

        public class EditorModeModule : ViewerModuleBase, IViewerInteractiveMode
        {
            private InterfaceRoot _interfaceRoot;

            public string Name => "Editor";
            public object IconKey => CalameIconKey.CursorMode;
            public Cursor Cursor => Cursors.Cross;
            public bool UseFreeCamera => true;

            public GlyphObject Root { get; private set; }

            public InteractiveInterfaceRoot Interactive => _interfaceRoot.Interactive;
            IInteractive IViewerInteractiveMode.Interactive => Interactive;

            protected override void ConnectRunner()
            {
                _interfaceRoot = Model.EditorRoot.Add<InterfaceRoot>();
                _interfaceRoot.RaycastClient = Model.Client;

                Root = Model.EditorRoot.Add<GlyphObject>();
                Root.Name = "Editor Mode Root";
                Root.Add<UserInterface>();

                Model.AddInteractiveMode(this);
            }

            protected override void DisconnectRunner()
            {
                Model.RemoveInteractiveMode(this);
                
                Model.EditorRoot.RemoveAndDispose(Root);
                Root = null;

                Model.EditorRoot.RemoveAndDispose(_interfaceRoot);
                _interfaceRoot = null;
            }
        }
    }
}