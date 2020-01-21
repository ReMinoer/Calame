using System.ComponentModel.Composition;
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
        private IDocumentContext<IComponentFilter> _filteringContext;
        public override PaneLocation PreferredLocation => PaneLocation.Left;

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
                SetValue(ref _selection, value);
                EventAggregator.PublishOnUIThread(new SelectionRequest<IGlyphComponent>(CurrentDocument, _selection));
            }
        }

        [ImportingConstructor]
        public CompositionGraphViewModel(IShell shell, IEventAggregator eventAggregator)
            : base(shell, eventAggregator)
        {
            DisplayName = "Composition Graph";
        }

        protected override void OnDocumentActivated(IDocumentContext<GlyphEngine> activeDocument)
        {
            Root = activeDocument.Context.Root;
            _filteringContext = activeDocument as IDocumentContext<IComponentFilter>;
        }

        protected override void OnDocumentsCleaned()
        {
            Root = null;
            _filteringContext = null;
        }

        void IHandle<ISelectionSpread<IGlyphComponent>>.Handle(ISelectionSpread<IGlyphComponent> message) => HandleSelection(message.Item);
        void IHandle<ISelectionSpread<IGlyphData>>.Handle(ISelectionSpread<IGlyphData> message) => HandleSelection(message.Item?.BindedObject);
        private void HandleSelection(IGlyphComponent component) => SetValue(ref _selection, component, nameof(Selection));
        
        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data)
        {
            var component = (IGlyphComponent)data;

            return new TreeViewItemModel<IGlyphComponent>(
                this,
                component,
                x => x.Name,
                x => new EnumerableReadOnlyObservableList<object>(x.Components),
                nameof(IGlyphComponent.Name),
                nameof(IGlyphComponent.Components))
            {
                IsEnabled = _filteringContext?.Context.Filter(component) ?? true
            };
        }
        
        bool ITreeContext.BaseFilter(object data) => true;
    }
}