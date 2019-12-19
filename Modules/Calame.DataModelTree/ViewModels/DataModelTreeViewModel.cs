using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph.Composition.Modelization;

namespace Calame.DataModelTree.ViewModels
{
    [Export(typeof(DataModelTreeViewModel))]
    public sealed class DataModelTreeViewModel : HandleTool, IHandle<IDocumentContext<IGlyphData>>, IHandle<ISelectionSpread<IGlyphData>>
    {
        private IGlyphData _root;
        private IGlyphData _selection;
        public override PaneLocation PreferredLocation => PaneLocation.Left;

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
                SetValue(ref _selection, value);

                if (_selection != null)
                {
                    EventAggregator.PublishOnUIThread(new SelectionRequest<IGlyphData>(CurrentDocument, _selection));
                }
            }
        }

        [ImportingConstructor]
        public DataModelTreeViewModel(IShell shell, IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            DisplayName = "Data Model Tree";

            if (shell.ActiveItem is IDocumentContext<IGlyphData> documentContext)
                Root = documentContext.Context;
        }

        void IHandle<IDocumentContext<IGlyphData>>.Handle(IDocumentContext<IGlyphData> message) => Root = message.Context;
        void IHandle<ISelectionSpread<IGlyphData>>.Handle(ISelectionSpread<IGlyphData> message) => SetValue(ref _selection, message.Item, nameof(Selection));
    }
}