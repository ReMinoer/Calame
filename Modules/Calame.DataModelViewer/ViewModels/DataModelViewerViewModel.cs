using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Calame.Viewer;
using Calame.Viewer.Modules;
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
    public class DataModelViewerViewModel : PersistedDocument, IViewerViewModelOwner, IDocumentContext<GlyphEngine>, IDocumentContext<IGlyphCreator>, IDisposable
    {
        private readonly IContentManagerProvider _contentManagerProvider;
        private readonly IEventAggregator _eventAggregator;

        private readonly ViewerViewModel _viewerViewModel;
        private readonly BoxedComponentSelectorModule _boxedComponentSelectorModule;

        private GlyphEngine _engine;

        public IEditor Editor { get; set; }
        public IGlyphCreator Data { get; private set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => _viewerViewModel.Runner?.Engine;
        IGlyphCreator IDocumentContext<IGlyphCreator>.Context => Data;

        public DataModelViewerViewModel(IContentManagerProvider contentManagerProvider, IEventAggregator eventAggregator)
        {
            _contentManagerProvider = contentManagerProvider;
            _eventAggregator = eventAggregator;
            
            var viewerModules = new IViewerModule[]
            {
                new SceneNodeEditorModule(eventAggregator),
                _boxedComponentSelectorModule = new BoxedComponentSelectorModule(eventAggregator),
                new SelectionRendererModule(eventAggregator)
            };

            _viewerViewModel = new ViewerViewModel(this, eventAggregator, viewerModules);
            _viewerViewModel.RunnerChanged += ViewerViewModelOnRunnerChanged;

            _boxedComponentSelectorModule.SelectionChanged += BoxedComponentSelectorModuleOnSelectionChanged;
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
            _engine = new GlyphEngine(_contentManagerProvider.Get(Editor.ContentPath));
            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            _viewerViewModel.Runner = new GlyphWpfRunner { Engine = _engine };
            
            Data.Resolver = _engine.Resolver;
            Data.Instantiate();

            IGlyphComposite<IGlyphComponent> dataRoot = Editor.PrepareEditor(_viewerViewModel.Runner.Engine, _viewerViewModel.EditorRoot);
            dataRoot.Add(Data.BindedObject);

            _engine.Initialize();
            _engine.LoadContent();
            _engine.Start();

            _viewerViewModel.EditorSessionInteractive.EditionMode = true;
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

        private void BoxedComponentSelectorModuleOnSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            _eventAggregator.PublishOnUIThread(Selection.Of(Data.GetData(boxedComponent)));
        }

        public void Dispose()
        {
            _engine.Stop();
            
            _boxedComponentSelectorModule.SelectionChanged -= BoxedComponentSelectorModuleOnSelectionChanged;
            _viewerViewModel.RunnerChanged -= ViewerViewModelOnRunnerChanged;
            Activated -= OnActivated;

            Data.Dispose();
            _viewerViewModel.Dispose();
        }
    }
}