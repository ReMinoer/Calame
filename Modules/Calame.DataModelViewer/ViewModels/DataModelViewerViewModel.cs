using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DataModelViewer.Views;
using Caliburn.Micro;
using Diese.Collections;
using Fingear.Controls;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Gemini.Framework;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Glyph.Modelization;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stave;
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.DataModelViewer.ViewModels
{
    [Export(typeof(DataModelViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataModelViewerViewModel : PersistedDocument, IDocumentContext<GlyphEngine>, IDisposable
    {
        private readonly ContentManagerProvider _contentManagerProvider;
        private readonly IEventAggregator _eventAggregator;
        private DataModelViewerView _view;
        private Cursor _viewerCursor;
        private GlyphWpfRunner _runner;
        private ShapedObjectSelector _shapedObjectSelector;
        private AreaComponentRenderer _selectionRenderer;
        private readonly Dictionary<IGlyphComponent, IGlyphCreator<IGlyphComponent>> _dataBindings = new Dictionary<IGlyphComponent, IGlyphCreator<IGlyphComponent>>();
        public IEditor Editor { get; set; }
        public IBindedGlyphCreator<IGlyphComponent> Data { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        public Glyph.Graphics.View EditorView { get; private set; }
        public GlyphWpfViewer Viewer { get; private set; }
        GlyphEngine IDocumentContext<GlyphEngine>.Context => Runner?.Engine;

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    _runner.Engine.ViewManager.UnregisterView(EditorView);
                    _runner.Engine.Root.RemoveAndDispose(EditorCamera);
                }
                
                _runner = value;

                if (_runner != null)
                {
                    _shapedObjectSelector = _runner.Engine.Root.Add<ShapedObjectSelector>();
                    _shapedObjectSelector.Control = new HybridControl<System.Numerics.Vector2>("Pointer")
                    {
                        TriggerControl = new Control(InputSystem.Instance.Mouse[MouseButton.Left]),
                        ValueControl = new SceneCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, _runner.Engine.InputClientManager, _runner.Engine.ViewManager)
                    };
                    _shapedObjectSelector.HandleInputs = true;
                    _shapedObjectSelector.SelectionChanged += ShapedObjectSelectorOnSelectionChanged;

                    EditorView = _runner.Engine.Injector.Resolve<Glyph.Graphics.View>();
                    EditorView.Name = "Editor View";
                    EditorView.BoundingBox = new TopLeftRectangle(Vector2.Zero, VirtualResolution.Size);
                    EditorView.DrawClientFilter = new ExcludingFilter<IDrawClient>();

                    EditorCamera = _runner.Engine.Root.Add<FreeCamera>();
                    EditorCamera.View = EditorView;
                    _runner.Engine.ViewManager.RegisterView(EditorView);

                    _runner.Engine.Root.Schedulers.Update.Plan(EditorCamera).AtStart();
                }
                
                ConnectView();

                if (IsActive)
                    OnActivated();
            }
        }

        public Cursor ViewerCursor
        {
            get => _viewerCursor;
            set
            {
                if (_viewerCursor == value)
                    return;

                _viewerCursor = value;
                NotifyOfPropertyChange();
            }
        }

        public DataModelViewerViewModel(ContentManagerProvider contentManagerProvider, IEventAggregator eventAggregator)
        {
            _contentManagerProvider = contentManagerProvider;
            _eventAggregator = eventAggregator;
        }

        protected override async Task DoNew()
        {
            Data = await Editor.NewDataAsync();
            InitializeEngine();
        }

        protected override async Task DoLoad(string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
                Data = await Editor.LoadDataAsync(fileStream);
            InitializeEngine();
        }

        private void InitializeEngine()
        {
            Runner = new GlyphWpfRunner
            {
                Engine = new GlyphEngine(_contentManagerProvider.Get(Editor.ContentPath))
            };

            Data.Injector = Runner.Engine.Injector;
            Data.Instantiate();
            _dataBindings[Data.BindedObject] = Data;
            
            Editor.PrepareEditor(Runner.Engine).Add(Data.BindedObject);
        }

        protected override async Task DoSave(string filePath)
        {
            using (FileStream fileStream = File.OpenWrite(filePath))
                await Editor.SaveDataAsync(Data, fileStream);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (DataModelViewerView)view;
            Viewer = _view.GlyphWpfViewer;

            ConnectView();

            Activated += OnActivated;
            Deactivated += OnDeactivated;
        }

        private void ConnectView()
        {
            if (_view == null)
                return;
            
            if (Runner == null)
                return;
            
            Viewer.Runner = Runner;
            EditorCamera.Client = Viewer;
            Runner.Engine.FocusedClient = Viewer;
        }

        private void ShapedObjectSelectorOnSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (_selectionRenderer != null)
            {
                Runner.Engine.Root.Remove(_selectionRenderer);
                _selectionRenderer.Dispose();
            }

            if (boxedComponent != null)
            {
                _selectionRenderer = new AreaComponentRenderer(boxedComponent, Runner.Engine.Injector.Resolve<Func<GraphicsDevice>>())
                {
                    Name = "Selection Renderer",
                    Color = Color.Purple * 0.5f,
                    DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera
                };
                Runner.Engine.Root.Add(_selectionRenderer);
            }

            if (boxedComponent == null)
            {
                _eventAggregator.PublishOnUIThread(Selection<IGlyphCreator<IGlyphComponent>>.Empty);
                return;
            }

            if (_dataBindings.TryGetValue(boxedComponent, out IGlyphCreator<IGlyphComponent> data))
                _eventAggregator.PublishOnUIThread(new Selection<IGlyphCreator<IGlyphComponent>>(data));
            else
            {
                if (boxedComponent.ParentQueue().Any(x => _dataBindings.ContainsKey(x), out IGlyphContainer parent))
                {
                    data = _dataBindings[parent];
                    _eventAggregator.PublishOnUIThread(new Selection<IGlyphCreator<IGlyphComponent>>(data));
                }
                else
                    _eventAggregator.PublishOnUIThread(Selection<IGlyphCreator<IGlyphComponent>>.Empty);
            }
        }

        public void Dispose()
        {
            Activated -= OnActivated;
            Deactivated -= OnDeactivated;

            (Data as IDisposable)?.Dispose();

            Runner?.Dispose();
            Runner = null;
        }

        private void OnActivated()
        {
            if (Runner?.Engine != null)
                Runner.Engine.FocusedClient = Viewer;

            _eventAggregator.PublishOnUIThread(new DocumentContext<GlyphEngine>(_runner?.Engine));
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => OnActivated();

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            if (Runner?.Engine != null && Runner.Engine.FocusedClient == Viewer)
                Runner.Engine.FocusedClient = null;
        }
    }
}