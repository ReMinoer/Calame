using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
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

namespace Calame.Viewer
{
    public class ViewerViewModel : PropertyChangedBase, IDisposable
    {
        private readonly IViewerViewModelOwner _owner;
        private readonly IEventAggregator _eventAggregator;
        
        private GlyphWpfRunner _runner;
        private IInteractive _editorInteractive;
        private InteractiveToggle _interactiveToggle;

        public IWpfGlyphClient Client { get; private set; }
        public FillView EditorView { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        public GlyphObject EditorRoot { get; private set; }

        public ReadOnlyCollection<IViewerModule> Modules { get; }
        public ReadOnlyObservableCollection<IViewerInteractiveMode> InteractiveModes { get; }
        private readonly ObservableCollection<IViewerInteractiveMode> _interactiveModes;
        
        public ComponentFilter ComponentsFilter { get; }
        public ISelectionSpread<object> LastSelection { get; set; }

        private IViewerInteractiveMode _selectedNode;
        public IViewerInteractiveMode SelectedMode
        {
            get => _selectedNode;
            set
            {
                if (!this.SetValue(ref _selectedNode, value))
                    return;

                _interactiveToggle.SelectedInteractive = SelectedMode.Interactive;
                Cursor = SelectedMode.Cursor;
                EditorCamera.Enabled = SelectedMode.UseFreeCamera;
            }
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
                    
                    engine.InteractionManager.Root.Remove(_editorInteractive);
                    engine.InteractionManager.Root.Remove(_interactiveToggle);
                    _runner.Engine.Root.RemoveAndDispose(EditorRoot);

                    _editorInteractive = null;
                    EditorView = null;
                    EditorCamera = null;
                    EditorRoot = null;

                    _interactiveToggle = null;
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

                    _interactiveToggle = new InteractiveToggle();
                    engine.InteractionManager.Root.Add(_interactiveToggle);

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

            Modules = new ReadOnlyCollection<IViewerModule>(moduleSources.Where(x => x.IsValidForDocument(owner)).Select(x => x.CreateInstance()).ToArray());

            _interactiveModes = new ObservableCollection<IViewerInteractiveMode>();
            InteractiveModes = new ReadOnlyObservableCollection<IViewerInteractiveMode>(_interactiveModes);

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
            _interactiveModes.Add(interactiveMode);
            _interactiveToggle.Add(interactiveMode.Interactive);
        }

        public void RemoveInteractiveMode(IViewerInteractiveMode interactiveMode)
        {
            _interactiveToggle.Remove(interactiveMode.Interactive);
            _interactiveModes.Remove(interactiveMode);
        }
        
        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => Activate();
        public void Activate()
        {
            if (Runner?.Engine == null)
                return;

            Runner.Engine.FocusedClient = Client;
            _eventAggregator.PublishOnUIThread(_owner);

            if (LastSelection != null)
                _eventAggregator.PublishOnUIThread(LastSelection);
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

            foreach (IViewerModule module in Modules)
                module.Disconnect();

            Runner?.Dispose();
            Runner = null;
        }
    }
}