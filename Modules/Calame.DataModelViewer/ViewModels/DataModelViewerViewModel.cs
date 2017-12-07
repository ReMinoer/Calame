using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DataModelViewer.Views;
using Caliburn.Micro;
using Diese.Collections;
using Gemini.Framework;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Glyph.Math.Shapes;
using Glyph.Tools;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework;

namespace Calame.DataModelViewer.ViewModels
{
    [Export(typeof(DataModelViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class DataModelViewerViewModel : PersistedDocument, IDisposable
    {
        private readonly ContentManagerProvider _contentManagerProvider;
        private DataModelViewerView _view;
        private Cursor _viewerCursor;
        private GlyphWpfRunner _runner;
        public IEditor Editor { get; set; }
        public IGlyphCreator<IGlyphComponent> Data { get; private set; }
        public FreeCamera EditorCamera { get; private set; }
        public Glyph.Graphics.View EditorView { get; private set; }
        public GlyphWpfViewer Viewer { get; private set; }

        public GlyphWpfRunner Runner
        {
            get => _runner;
            set
            {
                if (_runner != null)
                {
                    ViewManager.Main.UnregisterView(EditorView);
                    _runner.Engine.Root.RemoveAndDispose(EditorCamera);
                }
                
                _runner = value;

                if (_runner != null)
                {
                    EditorCamera = _runner.Engine.Root.Add<FreeCamera>();
                    EditorView = _runner.Engine.Injector.Resolve<Glyph.Graphics.View>();
                    EditorView.Name = "Editor View";
                    EditorView.BoundingBox = new TopLeftRectangle(Vector2.Zero, VirtualResolution.Size);
                    EditorView.DrawClientFilter = new ExcludingFilter<IDrawClient>();

                    EditorCamera.View = EditorView;
                    ViewManager.Main.RegisterView(EditorView);

                    _runner.Engine.Root.Schedulers.Update.Plan(EditorCamera).AtStart();
                }
                
                ConnectView();
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

        public DataModelViewerViewModel(ContentManagerProvider contentManagerProvider)
        {
            _contentManagerProvider = contentManagerProvider;
        }

        private void InitializeEngine()
        {
            Runner = new GlyphWpfRunner
            {
                Engine = new GlyphEngine(_contentManagerProvider.Get(Editor.ContentPath))
            };
        }

        protected override async Task DoNew()
        {
            InitializeEngine();
            
            Data = await Editor.NewDataAsync();
            Data.Injector = Runner.Engine.Injector;
            Editor.PrepareEditor(Runner.Engine).Add(Data.Create());
        }

        protected override async Task DoLoad(string filePath)
        {
            InitializeEngine();

            using (FileStream fileStream = File.OpenRead(filePath))
            {
                Data = await Editor.LoadDataAsync(fileStream);
                Data.Injector = Runner.Engine.Injector;
                Editor.PrepareEditor(Runner.Engine).Add(Data.Create());
            }
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

        public void Dispose()
        {
            Activated -= OnActivated;
            
            Runner?.Dispose();
            Runner = null;
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs)
        {
            if (Runner?.Engine != null)
                Runner.Engine.FocusedClient = Viewer;
        }
    }
}