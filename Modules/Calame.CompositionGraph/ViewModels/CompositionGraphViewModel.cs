using System.ComponentModel.Composition;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Engine;

namespace Calame.CompositionGraph.ViewModels
{
    [Export(typeof(CompositionGraphViewModel))]
    public sealed class CompositionGraphViewModel : HandleTool, IHandle<ISelection<IGlyphComponent>>, IHandle<IDocumentContext<GlyphEngine>>, ITreeContext
    {
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
                EventAggregator.PublishOnUIThread(new Selection<IGlyphComponent>(_selection));
            }
        }

        [ImportingConstructor]
        public CompositionGraphViewModel(IShell shell, IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            DisplayName = "Composition Graph";

            if (shell.ActiveItem is IDocumentContext<GlyphEngine> documentContext)
                Root = documentContext.Context.Root;
        }

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message) => SetValue(ref _selection, message.Item, nameof(Selection));
        void IHandle<IDocumentContext<GlyphEngine>>.Handle(IDocumentContext<GlyphEngine> message) => Root = message.Context.Root;
        
        public ITreeViewItemModel CreateTreeItemModel(object data)
        {
            return new TreeViewItemModel<IGlyphComponent>(
                this,
                (IGlyphComponent)data,
                x => x.Name,
                x => new EnumerableReadOnlyObservableList<object>(x.Components),
                nameof(IGlyphComponent.Name),
                nameof(IGlyphComponent.Components));
        }
    }
}