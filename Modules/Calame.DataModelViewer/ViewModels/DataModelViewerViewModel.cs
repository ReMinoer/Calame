using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Calame.DocumentContexts;
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
using Glyph.Resolver;
using Glyph.Tools.UndoRedo;
using Glyph.WpfInterop;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using Niddle;
using Simulacra.IO.Watching;

namespace Calame.DataModelViewer.ViewModels
{
    [Export(typeof(DataModelViewerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataModelViewerViewModel : CalamePersistedDocumentBase, IViewerDocument, IRunnableDocument,
        IDocumentContext<IRootDataContext>, IRootDataContext, IRootsContext,
        IDocumentContext<ISelectionContext<IGlyphData>>, ISelectionContext<IGlyphData>,
        IDocumentContext<IUndoRedoContext>, IUndoRedoContext,
        IHandle<ISelectionRequest<IGlyphData>>,
        IHandle<ISelectionRequest<IGlyphComponent>>
    {
        private readonly IImportedTypeProvider _importedTypeProvider;
        private readonly SelectionHistoryManager _selectionHistoryManager;

        private readonly DataModelUndoRedoManager _undoRedoManager;
        public IUndoRedoStack UndoRedoStack => _undoRedoManager;
        protected override sealed Gemini.Modules.UndoRedo.IUndoRedoManager CreateUndoRedoManager() => _undoRedoManager;

        private GlyphEngine _engine;
        public ViewerViewModel Viewer { get; }

        private IEditor _editor;
        private EditorContext _editorContext;

        public IEditor Editor
        {
            get => _editor;
            set
            {
                _editor = value;
                ToolBarDefinition = _editor?.ToolBarDefinition;
                RefreshIcon();
            }
        }
        
        Type IRunnableDocument.RunCommandDefinitionType => Editor?.RunCommandDefinitionType;
        Type IRunnableDocument.RunAlternativeCommandDefinitionType => Editor?.RunAlternativeCommandDefinitionType;

        IBoxedComponent IDefaultCameraTarget.DefaultCameraTarget => _editorContext.DefaultCameraTarget;
        void IViewerDocument.EnableFreeCamera() { }

        public ICommand DragOverCommand { get; }
        public ICommand DropCommand { get; }

        [ImportingConstructor]
        public DataModelViewerViewModel(IEventAggregator eventAggregator, ILoggerProvider loggerProvider, PathWatcher fileWatcher,
            IImportedTypeProvider importedTypeProvider, SelectionHistoryManager selectionHistoryManager,
            IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager,
            [ImportMany] IEnumerable<IViewerModuleSource> viewerModuleSources)
            : base(eventAggregator, loggerProvider, fileWatcher, iconProvider, iconDescriptorManager)
        {
            _importedTypeProvider = importedTypeProvider;
            _selectionHistoryManager = selectionHistoryManager;
            _undoRedoManager = new DataModelUndoRedoManager(this);

            Viewer = new ViewerViewModel(this, eventAggregator, viewerModuleSources);

            _debuggableViewerContexts = new DebuggableViewerContexts(Viewer);
            
            DragOverCommand = new RelayCommand(x => Editor.OnDragOver((DragEventArgs)x));
            DropCommand = new RelayCommand(x => Editor.OnDrop((DragEventArgs)x));
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
            RootData = new[] { Editor.Data };
            Roots = RootData;

            IGraphicsDeviceService graphicsDeviceService = WpfGraphicsDeviceService.Instance;
            IContentLibrary contentLibrary = Editor.CreateContentLibrary(graphicsDeviceService, Logger);

            _engine = new GlyphEngine(graphicsDeviceService, contentLibrary, Logger);

            _engine.Root.Add<SceneNode>();
            _engine.RootView.Camera = _engine.Root.Add<Camera>();
            
            Viewer.Runner = new GlyphWpfRunner { Engine = _engine };

            Editor.Data.DependencyResolver = _engine.Resolver;
            Editor.Data.SerializationKnownTypes = _importedTypeProvider.Types;

            _editorContext = new EditorContext(Viewer.Runner.Engine, Viewer.UserRoot, Viewer.UserRoot);
            Editor.PrepareEditor(_editorContext);

            _debuggableViewerContexts.RefreshDebuggableContexts();

            _engine.Initialize();
            await _engine.LoadContentAsync();
            
            Viewer.EditorCamera.ShowTarget(_editorContext.DefaultCameraTarget);

            _engine.Start();
            await Viewer.ActivateAsync();

            await EventAggregator.PublishAsync(new SelectionRequest<IGlyphData>(this, Editor.Data));

            IViewerInteractiveMode interactiveMode = Viewer.InteractiveModes.FirstOrDefault();
            if (interactiveMode != null)
                await EventAggregator.PublishAsync(new SwitchViewerModeRequest(this, interactiveMode));
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Viewer.ConnectView((IViewerView)view);

            CommandManager.InvalidateRequerySuggested();
        }

        protected override Task DisposeDocumentAsync()
        {
            EventAggregator.Unsubscribe(this);

            _engine.Stop();

            Editor.Dispose();
            Viewer.Dispose();

            _selectionHistoryManager.RemoveHistory(this);

            return Task.CompletedTask;
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

        private readonly DebuggableViewerContexts _debuggableViewerContexts;

        bool IViewerDocument.DebugMode
        {
            get => _debuggableViewerContexts.DebugMode;
            set => _debuggableViewerContexts.DebugMode = value;
        }

        IDocument IDocumentContext.Document => this;
        ViewerViewModel IDocumentContext<ViewerViewModel>.Context => _debuggableViewerContexts.Viewer;
        IContentLibraryContext IDocumentContext<IContentLibraryContext>.Context => _debuggableViewerContexts;
        IRawContentLibraryContext IDocumentContext<IRawContentLibraryContext>.Context => _debuggableViewerContexts;
        IRootComponentsContext IDocumentContext<IRootComponentsContext>.Context => _debuggableViewerContexts;
        IRootScenesContext IDocumentContext<IRootScenesContext>.Context => _debuggableViewerContexts;
        IViewsContext IDocumentContext<IViewsContext>.Context => _debuggableViewerContexts;
        IRootInteractivesContext IDocumentContext<IRootInteractivesContext>.Context => _debuggableViewerContexts;

        ISelectionContext IDocumentContext<ISelectionContext>.Context => this;
        ISelectionContext<IGlyphComponent> IDocumentContext<ISelectionContext<IGlyphComponent>>.Context => this;
        ISelectionContext<IGlyphData> IDocumentContext<ISelectionContext<IGlyphData>>.Context => this;
        IRootsContext IDocumentContext<IRootsContext>.Context => this;
        IRootDataContext IDocumentContext<IRootDataContext>.Context => this;
        IUndoRedoContext IDocumentContext<IUndoRedoContext>.Context => this;

        private IEnumerable _root;
        public IEnumerable Roots
        {
            get => _root;
            set => Set(ref _root, value);
        }

        private IEnumerable<IGlyphData> _rootData;
        public IEnumerable<IGlyphData> RootData
        {
            get => _rootData;
            set => Set(ref _rootData, value);
        }

        event EventHandler ISelectionContext.CanSelectChanged
        {
            add => _debuggableViewerContexts.CanSelectChanged += value;
            remove => _debuggableViewerContexts.CanSelectChanged -= value;
        }

        public bool CanSelect(object instance)
        {
            switch (instance)
            {
                case IGlyphData data:
                    return CanSelect(data);
                case IGlyphComponent component:
                    return CanSelect(component);
            }

            return false;
        }

        public bool CanSelect(IGlyphData data) => data is null || CanSelect(data.BindedObject);
        public bool CanSelect(IGlyphComponent component) => _debuggableViewerContexts.CanSelect(component);

        public Task SelectAsync(object instance)
        {
            switch (instance)
            {
                case IGlyphData data:
                    return SelectAsync(data);
                case IGlyphComponent component:
                    return SelectAsync(component);
            }

            return Task.CompletedTask;
        }

        public Task SelectAsync(IGlyphData data)
        {
            if (CanSelect(data))
                return EventAggregator.PublishAsync(new SelectionRequest<IGlyphData>(this, data));
            return Task.CompletedTask;
        }

        public Task SelectAsync(IGlyphComponent component)
        {
            if (CanSelect(component))
                return EventAggregator.PublishAsync(new SelectionRequest<IGlyphComponent>(this, component));
            return Task.CompletedTask;
        }
    }
}