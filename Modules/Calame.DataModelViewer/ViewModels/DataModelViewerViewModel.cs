using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Calame.Viewer;
using Caliburn.Micro;
using Gemini.Framework;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Glyph.WpfInterop;

namespace Calame.DataModelViewer.ViewModels
{
    [Export(typeof(DataModelViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataModelViewerViewModel : PersistedDocument, IViewerViewModelOwner, IDocumentContext<GlyphEngine>, IDocumentContext<ViewerViewModel>, IDocumentContext<IGlyphCreator>, IHandle<ISelection<IBoxedComponent>>, IDisposable
    {
        private readonly IContentLibraryProvider _contentLibraryProvider;
        private readonly IEventAggregator _eventAggregator;

        private readonly ViewerViewModel _viewerViewModel;

        private GlyphEngine _engine;

        public IEditor Editor { get; set; }
        public IGlyphCreator Data { get; private set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => _viewerViewModel.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => _viewerViewModel;
        IGlyphCreator IDocumentContext<IGlyphCreator>.Context => Data;
        
        [ImportingConstructor]
        public DataModelViewerViewModel(IContentLibraryProvider contentLibraryProvider, IEventAggregator eventAggregator, [ImportMany] IEnumerable<IViewerModule> viewerModules)
        {
            _contentLibraryProvider = contentLibraryProvider;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _viewerViewModel = new ViewerViewModel(this, eventAggregator, viewerModules);
            _viewerViewModel.RunnerChanged += ViewerViewModelOnRunnerChanged;
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

        protected override async Task DoSave(string filePath)
        {
            using (FileStream fileStream = File.Create(filePath))
                await Editor.SaveDataAsync(Data, fileStream);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _viewerViewModel.ConnectView((IViewerView)view);
        }

        private void InitializeEngine()
        {
            _engine = new GlyphEngine(_contentLibraryProvider.Get(Editor.ContentPath));
            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            _viewerViewModel.Runner = new GlyphWpfRunner { Engine = _engine };
            
            Data.DependencyResolver = _engine.Resolver;
            Data.Instantiate();

            IGlyphComposite<IGlyphComponent> dataRoot = Editor.PrepareEditor(_viewerViewModel.Runner.Engine, _viewerViewModel.EditorRoot);
            dataRoot.Add(Data.BindedObject);

            _engine.Initialize();
            _engine.LoadContent();
            _engine.Start();
        }

        private void OnActivated()
        {
            _eventAggregator.PublishOnUIThread(new DocumentContext<IGlyphCreator>(Data));
        }

        private void OnActivated(object sender, ActivationEventArgs activationEventArgs) => OnActivated();

        private void ViewerViewModelOnRunnerChanged(object sender, GlyphWpfRunner e)
        {
            OnActivated();
            Activated += OnActivated;
        }

        public void Handle(ISelection<IBoxedComponent> message)
        {
            _eventAggregator.PublishOnUIThread(Selection.Of(Data.GetData(message.Item)));
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
            _engine.Stop();
            
            _viewerViewModel.RunnerChanged -= ViewerViewModelOnRunnerChanged;
            Activated -= OnActivated;

            Data.Dispose();
            _viewerViewModel.Dispose();
        }
    }
}