using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Engine;

namespace Calame.CompositionGraph.ViewModels
{
    [Export(typeof(CompositionGraphViewModel))]
    public sealed class CompositionGraphViewModel : HandleTool, IHandle<ISelection<IGlyphComponent>>, IHandle<IDocumentContext<GlyphEngine>>
    {
        private GlyphEngine _engine;
        private IGlyphComponent _selection;
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public GlyphEngine Engine
        {
            get => _engine;
            private set => SetValue(ref _engine, value);
        }

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
                Engine = documentContext.Context;
        }
        
        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message) => SetValue(ref _selection, message.Item, nameof(Selection));
        void IHandle<IDocumentContext<GlyphEngine>>.Handle(IDocumentContext<GlyphEngine> message) => Engine = message.Context;
    }
}