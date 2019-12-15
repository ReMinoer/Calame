using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.Viewer;
using Caliburn.Micro;
using Gemini.Framework;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Glyph.WpfInterop;

namespace Calame.DataModelViewer.ViewModels
{
    [Export(typeof(DataModelViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataModelViewerViewModel : PersistedDocument, IViewerViewModelOwner, IDocumentContext<GlyphEngine>, IDocumentContext<ViewerViewModel>, IDocumentContext<IGlyphData>, IHandle<ISelection<IBoxedComponent>>, IDisposable
    {
        private readonly IContentLibraryProvider _contentLibraryProvider;
        private readonly IEventAggregator _eventAggregator;

        private GlyphEngine _engine;
        private Cursor _viewerCursor;
        private IViewerMode _selectedMode;
        
        public ViewerViewModel Viewer { get; }
        public IEditor Editor { get; set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => Viewer.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => Viewer;
        IGlyphData IDocumentContext<IGlyphData>.Context => Editor.Data;

        public Cursor ViewerCursor
        {
            get => _viewerCursor;
            set => this.SetValue(ref _viewerCursor, value);
        }

        public ICommand SwitchModeCommand { get; }
        
        [ImportingConstructor]
        public DataModelViewerViewModel(IContentLibraryProvider contentLibraryProvider, IEventAggregator eventAggregator, [ImportMany] IEnumerable<IViewerModule> viewerModules)
        {
            _contentLibraryProvider = contentLibraryProvider;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModules);
            Viewer.RunnerChanged += ViewerViewModelOnRunnerChanged;
            
            SwitchModeCommand = new RelayCommand(x => SwitchModeAction((IViewerMode)x), x => Viewer.Runner?.Engine != null);
        }

        protected override async Task DoNew()
        {
            await Editor.NewDataAsync();
            InitializeEngine();
        }

        protected override async Task DoLoad(string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
                await Editor.LoadDataAsync(fileStream);
            InitializeEngine();
        }

        protected override async Task DoSave(string filePath)
        {
            using (FileStream fileStream = File.Create(filePath))
                await Editor.SaveDataAsync(fileStream);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Viewer.ConnectView((IViewerView)view);
        }

        private void InitializeEngine()
        {
            _engine = new GlyphEngine(_contentLibraryProvider.Get(Editor.ContentPath));
            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            Viewer.Runner = new GlyphWpfRunner { Engine = _engine };

            Editor.Data.DependencyResolver = _engine.Resolver;
            Editor.RegisterDependencies(_engine.Registry);
            Editor.PrepareEditor(Viewer.Runner.Engine, Viewer.EditorRoot);

            _engine.Initialize();
            _engine.LoadContent();
            _engine.Start();
        }

        private void OnActivated()
        {
            _eventAggregator.PublishOnUIThread(new DocumentContext<IGlyphData>(Editor.Data));
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => OnActivated();

        private void ViewerViewModelOnRunnerChanged(object sender, GlyphWpfRunner e)
        {
            OnActivated();
            Activated += OnActivated;
        }

        public void Handle(ISelection<IBoxedComponent> message)
        {
            _eventAggregator.PublishOnUIThread(Selection.Of(Editor.Data.GetData(message.Item)));
        }

        private void SwitchModeAction(IViewerMode mode)
        {
            if (_selectedMode == mode)
                return;
            
            _selectedMode = mode;

            Viewer.InteractiveToggle.SelectedInteractive = mode.Interactive;
            ViewerCursor = mode.Cursor;
            Viewer.EditorCamera.Enabled = mode.UseFreeCamera;
        }

        public void Dispose()
        {
            Viewer.RunnerChanged -= ViewerViewModelOnRunnerChanged;
            Activated -= OnActivated;
            _eventAggregator.Unsubscribe(this);

            _engine.Stop();

            Editor.Dispose();
            Viewer.Dispose();
        }
    }
}