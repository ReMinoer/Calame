using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph.Engine;

namespace Calame.InteractionTree.ViewModels
{
    [Export(typeof(InteractionTreeViewModel))]
    public sealed class InteractionTreeViewModel : HandleTool, IHandle<IDocumentContext<GlyphEngine>>
    {
        private GlyphEngine _engine;
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public GlyphEngine Engine
        {
            get => _engine;
            private set => SetValue(ref _engine, value);
        }

        [ImportingConstructor]
        public InteractionTreeViewModel(IShell shell, IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            DisplayName = "Interaction Tree";

            if (shell.ActiveItem is IDocumentContext<GlyphEngine> documentContext)
                Engine = documentContext.Context;
        }
        
        void IHandle<IDocumentContext<GlyphEngine>>.Handle(IDocumentContext<GlyphEngine> message) => Engine = message.Context;
    }
}