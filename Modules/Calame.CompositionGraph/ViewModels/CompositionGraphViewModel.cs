using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Engine;

namespace Calame.CompositionGraph.ViewModels
{
    [Export(typeof(CompositionGraphViewModel))]
    public sealed class CompositionGraphViewModel : CalameTool<IDocumentContext<GlyphEngine>>, IHandle<ISelectionSpread<IGlyphComponent>>, IHandle<ISelectionSpread<IGlyphData>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;
        
        public IIconProvider IconProvider { get; }
        private readonly IIconDescriptor<IGlyphComponent> _iconDescriptor;
        
        private IDocumentContext<IComponentFilter> _filteringContext;

        private IGlyphComponent _root;
        public IGlyphComponent Root
        {
            get => _root;
            private set => SetValue(ref _root, value);
        }
        
        private IGlyphComponent _selection;
        public IGlyphComponent Selection
        {
            get => _selection;
            set
            {
                if (!SetValue(ref _selection, value))
                    return;

                var selectionRequest = new SelectionRequest<IGlyphComponent>(CurrentDocument, _selection);
                EventAggregator.PublishOnBackgroundThreadAsync(selectionRequest).Wait();
            }
        }

        [ImportingConstructor]
        public CompositionGraphViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator)
        {
            DisplayName = "Composition Graph";
            
            IconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();
        }

        protected override Task OnDocumentActivated(IDocumentContext<GlyphEngine> activeDocument)
        {
            _selection = null;

            Root = activeDocument.Context.Root;
            _filteringContext = activeDocument as IDocumentContext<IComponentFilter>;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            _selection = null;

            Root = null;
            _filteringContext = null;

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
        
        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data)
        {
            var component = (IGlyphComponent)data;

            return new TreeViewItemModel<IGlyphComponent>(
                this,
                component,
                x => x.Name,
                x => new EnumerableReadOnlyObservableList<object>(x.Components),
                _iconDescriptor.GetIcon(component),
                nameof(IGlyphComponent.Name),
                nameof(IGlyphComponent.Components))
            {
                IsEnabled = _filteringContext?.Context.Filter(component) ?? true
            };
        }
        
        bool ITreeContext.BaseFilter(object data) => true;
    }
}