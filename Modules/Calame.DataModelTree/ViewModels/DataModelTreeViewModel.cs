using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.DataModelTree.ViewModels
{
    [Export(typeof(DataModelTreeViewModel))]
    public sealed class DataModelTreeViewModel : HandleTool, IHandle<ISelection<IGlyphComponent>>, IHandle<IDocumentContext<IGlyphCreator>>
    {
        private IGlyphCreator _root;
        private IGlyphCreator _selection;
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IGlyphCreator Root
        {
            get => _root;
            private set => SetValue(ref _root, value);
        }

        public IGlyphCreator Selection
        {
            get => _selection;
            set
            {
                SetValue(ref _selection, value);

                if (_selection != null)
                {
                    EventAggregator.PublishOnUIThread(new Selection<IGlyphComponent>(_selection.BindedObject));
                    EventAggregator.PublishOnUIThread(new Selection<IGlyphCreator>(_selection));
                }
            }
        }

        [ImportingConstructor]
        public DataModelTreeViewModel(IShell shell, IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            DisplayName = "Data Model Tree";

            if (shell.ActiveItem is IDocumentContext<IGlyphCreator> documentContext)
                Root = documentContext.Context;
        }

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message) => SetValue(ref _selection, _root.GetData(message.Item), nameof(Selection));
        void IHandle<IDocumentContext<IGlyphCreator>>.Handle(IDocumentContext<IGlyphCreator> message) => Root = message.Context;
    }
}