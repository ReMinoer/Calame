using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Calame.Icons;
using Calame.Viewer;
using Calame.Viewer.Messages;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Gemini.Framework;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Glyph.WpfInterop;
using Microsoft.Xna.Framework.Graphics;
using Simulacra.IO.Watching;

namespace Calame.DataModelViewer.ViewModels
{
    [Export(typeof(DataModelViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataModelViewerViewModel : CalamePersistedDocumentBase, IViewerDocument, IDocumentContext<IGlyphData>, IHandle<ISelectionRequest<IGlyphData>>, IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private readonly IImportedTypeProvider _importedTypeProvider;

        private GlyphEngine _engine;
        
        public ViewerViewModel Viewer { get; }
        public IEditor Editor { get; set; }

        GlyphEngine IDocumentContext<GlyphEngine>.Context => Viewer.Runner?.Engine;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => Viewer;
        IComponentFilter IDocumentContext<IComponentFilter>.Context => Viewer.ComponentsFilter;
        IGlyphData IDocumentContext<IGlyphData>.Context => Editor.Data;

        public ICommand SwitchModeCommand { get; }

        public ICommand DragOverCommand { get; }
        public ICommand DropCommand { get; }

        public IIconProvider IconProvider { get; }
        public IIconDescriptor CalameIconDescriptor { get; }

        public string WorkingDirectory => _engine?.ContentLibrary?.WorkingDirectory;

        [ImportingConstructor]
        public DataModelViewerViewModel(IEventAggregator eventAggregator, PathWatcher fileWatcher, IImportedTypeProvider importedTypeProvider,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
            : base(eventAggregator, fileWatcher)
        {
            _importedTypeProvider = importedTypeProvider;

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModuleSources);
            
            SwitchModeCommand = new RelayCommand(OnSwitchMode, x => Viewer.Runner?.Engine != null);

            DragOverCommand = new RelayCommand(x => Editor.OnDragOver((DragEventArgs)x));
            DropCommand = new RelayCommand(x => Editor.OnDrop((DragEventArgs)x));

            IconProvider = iconProvider;
            CalameIconDescriptor = iconDescriptorManager.GetDescriptor<CalameIconKey>();
        }

        protected override async Task NewDocumentAsync()
        {
            await Editor.NewDataAsync();
            await InitializeEngineAsync();
        }

        protected override async Task LoadDocumentAsync(string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
                await Editor.LoadDataAsync(fileStream);
            await InitializeEngineAsync();
        }

        protected override async Task SaveDocumentAsync(string filePath)
        {
            using (FileStream fileStream = File.Create(filePath))
                await Editor.SaveDataAsync(fileStream);
        }

        private async Task InitializeEngineAsync()
        {
            IGraphicsDeviceService graphicsDeviceService = WpfGraphicsDeviceService.Instance;
            IContentLibrary contentLibrary = Editor.CreateContentLibrary(graphicsDeviceService);

            _engine = new GlyphEngine(graphicsDeviceService, contentLibrary);

            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            Viewer.Runner = new GlyphWpfRunner { Engine = _engine };

            Editor.Data.DependencyResolver = _engine.Resolver;
            Editor.Data.SerializationKnownTypes = _importedTypeProvider.Types;

            Editor.RegisterDependencies(_engine.Registry);
            Editor.PrepareEditor(Viewer.Runner.Engine, Viewer.UserRoot);

            _engine.Initialize();
            await _engine.LoadContentAsync();

            if (Editor.Data.BindedObject is IBoxedComponent boxedComponent)
            {
                Viewer.EditorCamera.ShowTarget(boxedComponent);
                Viewer.EditorCamera.SaveAsDefault();
            }

            _engine.Start();
            await Viewer.Activate();

            await EventAggregator.PublishAsync(new SelectionRequest<IGlyphData>(this, Editor.Data));

            IViewerInteractiveMode interactiveMode = Viewer.InteractiveModes.FirstOrDefault()?.InteractiveModel;
            if (interactiveMode != null)
                await EventAggregator.PublishAsync(new SwitchViewerModeRequest(this, interactiveMode));
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Viewer.ConnectView((IViewerView)view);
        }

        protected override Task DisposeDocumentAsync()
        {
            EventAggregator.Unsubscribe(this);

            _engine.Stop();

            Editor.Dispose();
            Viewer.Dispose();

            return Task.CompletedTask;
        }

        private async void OnSwitchMode(object obj)
        {
            await EventAggregator.PublishAsync(new SwitchViewerModeRequest(this, (IViewerInteractiveMode)obj));
        }

        async Task IHandle<ISelectionRequest<IGlyphData>>.HandleAsync(ISelectionRequest<IGlyphData> message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext != this)
                return;

            ISelectionSpread<IGlyphData> selection = message.Promoted;

            Viewer.LastSelection = selection;
            await EventAggregator.PublishAsync(selection, cancellationToken);
        }

        async Task IHandle<ISelectionRequest<IGlyphComponent>>.HandleAsync(ISelectionRequest<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            if (message.DocumentContext != this)
                return;

            ISelectionSpread<IGlyphData> selection = new SelectionSpread<IGlyphData>(message.DocumentContext, Editor.Data.GetData(message.Item));
            
            Viewer.LastSelection = selection;
            await EventAggregator.PublishAsync(selection, cancellationToken);
        }
    }
}