using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
    public class DataModelViewerViewModel : PersistedDocument, IViewerDocument, IDocumentContext<IGlyphData>, IHandle<ISelectionRequest<IGlyphData>>, IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private readonly IContentLibraryProvider _contentLibraryProvider;
        private readonly IEventAggregator _eventAggregator;

        private GlyphEngine _engine;
        
        public ViewerViewModel Viewer { get; }
        public IEditor Editor { get; set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => Viewer.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => Viewer;
        IComponentFilter IDocumentContext<IComponentFilter>.Context => Viewer.ComponentsFilter;
        IGlyphData IDocumentContext<IGlyphData>.Context => Editor.Data;

        public ICommand SwitchModeCommand { get; }
        
        [ImportingConstructor]
        public DataModelViewerViewModel(IContentLibraryProvider contentLibraryProvider, IEventAggregator eventAggregator, [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
        {
            _contentLibraryProvider = contentLibraryProvider;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModuleSources);
            
            SwitchModeCommand = new RelayCommand(x => Viewer.SelectedMode = (IViewerInteractiveMode)x, x => Viewer.Runner?.Engine != null);
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
            
            Viewer.SelectedMode = Viewer.InteractiveModes.FirstOrDefault();
            Viewer.Activate();
        }

        public void Handle(ISelectionRequest<IGlyphData> message)
        {
            if (message.DocumentContext != this)
                return;

            ISelectionSpread<IGlyphData> selection = message.Promoted;

            Viewer.LastSelection = selection;
            _eventAggregator.PublishOnUIThread(selection);
        }

        public void Handle(ISelectionRequest<IGlyphComponent> message)
        {
            if (message.DocumentContext != this)
                return;

            ISelectionSpread<IGlyphData> selection = new SelectionSpread<IGlyphData>(message.DocumentContext, Editor.Data.GetData(message.Item));
            
            Viewer.LastSelection = selection;
            _eventAggregator.PublishOnUIThread(selection);
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);

            _engine.Stop();

            Editor.Dispose();
            Viewer.Dispose();
        }
    }
}