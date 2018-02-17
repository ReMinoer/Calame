using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DataModelViewer.Views;
using Caliburn.Micro;
using Diese.Collections;
using Fingear.Controls;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MouseButton = Fingear.MonoGame.Inputs.MouseButton;

namespace Calame.DataModelViewer.ViewModels
{
    [Export(typeof(DataModelViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataModelViewerViewModel : HandlePersistedDocument, IDocumentContext<GlyphEngine>, IDocumentContext<IGlyphCreator>, IHandle<ISelection<IGlyphComponent>>, IDisposable
    {
        private readonly IContentManagerProvider _contentManagerProvider;
        private readonly IImportedTypeProvider _importedTypeProvider;
        private DataModelViewerView _view;
        private Cursor _viewerCursor;
        private GlyphWpfRunner _runner;
        private IBoxedComponent _boxedSelection;
        private ShapedObjectSelector _shapedObjectSelector;
        private AreaComponentRenderer _selectionRenderer;
        public IEditor Editor { get; set; }
        public IGlyphCreator Data { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        public Glyph.Graphics.View EditorView { get; private set; }
        public GlyphWpfViewer Viewer { get; private set; }
        GlyphEngine IDocumentContext<GlyphEngine>.Context => Runner?.Engine;
        IGlyphCreator IDocumentContext<IGlyphCreator>.Context => Data;

        public IBoxedComponent BoxedSelection
        {
            get => _boxedSelection;
            private set
            {
                if (SetValue(ref _boxedSelection, value))
                {
                    if (_selectionRenderer != null)
                    {
                        Runner.Engine.Root.Remove(_selectionRenderer);
                        _selectionRenderer.Dispose();
                    }

                    _boxedSelection = value;

                    if (_boxedSelection != null)
                    {
                        _selectionRenderer = new AreaComponentRenderer(_boxedSelection, Runner.Engine.Injector.Resolve<Func<GraphicsDevice>>()) { Name = "Selection Renderer", Color = Color.Purple * 0.5f, DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera };
                        Runner.Engine.Root.Add(_selectionRenderer);
                    }
                }
            }
        }

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

                if (IsActive && Data.IsInstantiated)
                    OnActivated();
            }
        }

        public Cursor ViewerCursor
        {
            get => _viewerCursor;
            set => SetValue(ref _viewerCursor, value);
        }

        public DataModelViewerViewModel(IContentManagerProvider contentManagerProvider, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider)
            : base(eventAggregator)
        {
            _contentManagerProvider = contentManagerProvider;
            _importedTypeProvider = importedTypeProvider;
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
            Runner = new GlyphWpfRunner { Engine = new GlyphEngine(_contentManagerProvider.Get(Editor.ContentPath)) };

            Data.Injector = Runner.Engine.Injector;
            Data.Instantiate();

            Editor.PrepareEditor(Runner.Engine).Add(Data.BindedObject);
            OnActivated();
        }

        protected override async Task DoSave(string filePath)
        {
            using (FileStream fileStream = File.Create(filePath))
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

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message)
        {
            if (Runner.Engine.FocusedClient == Viewer && message.Item is IBoxedComponent boxedComponent)
                BoxedSelection = boxedComponent;
        }

        private void ShapedObjectSelectorOnSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (Runner.Engine.FocusedClient == Viewer)
            {
                BoxedSelection = boxedComponent;
                EventAggregator.PublishOnUIThread(Selection.New(_boxedSelection));
                EventAggregator.PublishOnUIThread(Selection.New(Data.GetData(_boxedSelection)));
            }
        }

        public void Dispose()
        {
            Activated -= OnActivated;
            Deactivated -= OnDeactivated;

            Data.Dispose();

            Runner?.Dispose();
            Runner = null;
        }

        private void OnActivated()
        {
            if (Runner?.Engine != null)
                Runner.Engine.FocusedClient = Viewer;

            EventAggregator.PublishOnUIThread(new DocumentContext<GlyphEngine>(_runner?.Engine));
            EventAggregator.PublishOnUIThread(new DocumentContext<IGlyphCreator>(Data));
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => OnActivated();

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            if (Runner?.Engine != null && Runner.Engine.FocusedClient == Viewer)
                Runner.Engine.FocusedClient = null;
        }
    }
}