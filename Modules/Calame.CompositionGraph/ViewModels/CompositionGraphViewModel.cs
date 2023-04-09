using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Calame.DocumentContexts;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.CompositionGraph.ViewModels
{
    [Export(typeof(CompositionGraphViewModel))]
    public sealed class CompositionGraphViewModel : CalameTool<IDocumentContext<IRootComponentsContext>>, ITreeContext,
        IHandle<ISelectionSpread<IGlyphComponent>>,
        IHandle<ISelectionSpread<IGlyphData>>
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;
        
        public IIconProvider IconProvider { get; }
        public IIconDescriptor IconDescriptor { get; }
        
        private readonly TreeViewItemModelBuilder<IGlyphComponent> _treeItemBuilder;
        private IUndoRedoContext _undoRedoContext;
        private ISelectionContext<IGlyphComponent> _selectionContext;
        private ICommand _selectionCommand;

        private IRootComponentsContext _rootComponentsContext;
        public IRootComponentsContext RootComponentsContext
        {
            get => _rootComponentsContext;
            private set => SetValue(ref _rootComponentsContext, value);
        }
        
        private IGlyphComponent _selection;
        public IGlyphComponent Selection
        {
            get => _selection;
            set
            {
                if (!SetValue(ref _selection, value))
                    return;

                _selectionContext.SelectAsync(_selection).Wait();
            }
        }

        protected override object IconKey => CalameIconKey.CompositionGraph;

        [ImportingConstructor]
        public CompositionGraphViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator, iconProvider, iconDescriptorManager)
        {
            DisplayName = "Composition Graph";
            
            IconProvider = iconProvider;
            IconDescriptor = iconDescriptorManager.GetDescriptor();

            IIconDescriptor<IGlyphComponent> iconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();

            _treeItemBuilder = new TreeViewItemModelBuilder<IGlyphComponent>()
                               .DisplayName(x => x.Name, nameof(IGlyphComponent.Name))
                               .CanEditDisplayName(_ => true)
                               .DisplayNameSetter(x => newName => x.Name = newName)
                               .ChildrenSource(x => new EnumerableReadOnlyObservableList<object>(x.Components), nameof(IGlyphComponent.Components))
                               .IconDescription(x => iconDescriptor.GetIcon(x))
                               .IsEnabled(_ => _selectionCommand);
        }

        protected override Task OnDocumentActivated(IDocumentContext<IRootComponentsContext> activeDocument)
        {
            _selection = null;

            _undoRedoContext = activeDocument.TryGetContext<IUndoRedoContext>();
            _selectionContext = activeDocument.GetSelectionContext<IGlyphComponent>();
            _selectionCommand = _selectionContext.GetSelectionCommand();

            RootComponentsContext = activeDocument.Context;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            _selection = null;

            RootComponentsContext = null;

            _selectionCommand = null;
            _selectionContext = null;
            _undoRedoContext = null;

            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphComponent>>.HandleAsync(ISelectionSpread<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item);
            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item?.BindedObject);
            return Task.CompletedTask;
        }

        private void HandleSelection(IGlyphComponent component) => SetValue(ref _selection, component, nameof(Selection));
        
        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data, ICollectionSynchronizerConfiguration<object, ITreeViewItemModel> synchronizerConfiguration)
        {
            return _treeItemBuilder.Build((IGlyphComponent)data, synchronizerConfiguration, _undoRedoContext?.UndoRedoStack);
        }

        public bool DisableChildrenIfParentDisabled => true;
        event EventHandler ITreeContext.BaseFilterChanged { add { } remove { } }
        bool ITreeContext.IsMatchingBaseFilter(object data) => true;
    }
}