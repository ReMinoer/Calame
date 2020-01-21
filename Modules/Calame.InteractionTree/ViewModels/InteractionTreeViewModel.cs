using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph.Engine;

namespace Calame.InteractionTree.ViewModels
{
    [Export(typeof(InteractionTreeViewModel))]
    public sealed class InteractionTreeViewModel : CalameTool<IDocumentContext<GlyphEngine>>
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
            : base(shell, eventAggregator)
        {
            DisplayName = "Interaction Tree";
        }
        
        protected override void OnDocumentActivated(IDocumentContext<GlyphEngine> activeDocument)
        {
            Engine = activeDocument.Context;
        }

        protected override void OnDocumentsCleaned()
        {
            Engine = null;
        }
    }
}