using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Diese.Collections.Observables.ReadOnly;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.DataModelTree.ViewModels
{
    [Export(typeof(DataModelTreeViewModel))]
    public sealed class DataModelTreeViewModel : CalameTool<IDocumentContext<IGlyphData>>, IHandle<ISelectionSpread<IGlyphData>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }
        private readonly IIconDescriptor<IGlyphComponent> _iconDescriptor;

        private IGlyphData _root;
        private IGlyphData _selection;

        public IGlyphData Root
        {
            get => _root;
            private set => SetValue(ref _root, value);
        }

        public IGlyphData Selection
        {
            get => _selection;
            set
            {
                if (SetValue(ref _selection, value))
                    EventAggregator.PublishOnUIThread(new SelectionRequest<IGlyphData>(CurrentDocument, _selection));
            }
        }

        [ImportingConstructor]
        public DataModelTreeViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator)
        {
            DisplayName = "Data Model Tree";

            IconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();
        }
        
        protected override void OnDocumentActivated(IDocumentContext<IGlyphData> activeDocument)
        {
            _selection = null;
            Root = activeDocument.Context;
        }

        protected override void OnDocumentsCleaned()
        {
            _selection = null;
            Root = null;
        }
        
        void IHandle<ISelectionSpread<IGlyphData>>.Handle(ISelectionSpread<IGlyphData> message) => SetValue(ref _selection, message.Item, nameof(Selection));
        
        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data)
        {
            var glyphCreator = (IGlyphCreator)data;

            return new TreeViewItemModel<IGlyphCreator>(
                this,
                glyphCreator,
                x => x.Name,
                x => new EnumerableReadOnlyObservableList<object>(x.Children),
                _iconDescriptor.GetIcon(glyphCreator.BindedObject),
                nameof(IGlyphCreator.Name),
                nameof(IGlyphCreator.Children));
        }
        
        bool ITreeContext.BaseFilter(object data) => true;
    }
}