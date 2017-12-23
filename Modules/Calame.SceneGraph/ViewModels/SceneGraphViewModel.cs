using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.SceneGraph.ViewModels
{
    [Export(typeof(SceneGraphViewModel))]
    public sealed class SceneGraphViewModel : HandleTool, IHandle<IDocumentContext<GlyphEngine>>, IHandle<ISelection<IGlyphComponent>>
    {
        private GlyphEngine _engine;
        private IGlyphComponent _selection;
        private ISceneNode _selectionNode;
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
                if (!SetValue(ref _selection, value))
                    return;

                _selectionNode = _selection?.GetSceneNode();
                NotifyOfPropertyChange(nameof(SelectionNode));
                    
                EventAggregator.PublishOnUIThread(new Selection<IGlyphComponent>(_selection));
            }
        }

        public ISceneNode SelectionNode
        {
            get => _selectionNode;
            set
            {
                if (!SetValue(ref _selectionNode, value))
                    return;

                _selection = _selectionNode?.Parent;
                NotifyOfPropertyChange(nameof(Selection));

                EventAggregator.PublishOnUIThread(new Selection<IGlyphComponent>(_selection));
            }
        }

        [ImportingConstructor]
        public SceneGraphViewModel(IShell shell, IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            DisplayName = "Scene Graph";

            if (shell.ActiveItem is IDocumentContext<GlyphEngine> documentContext)
                Engine = documentContext.Context;
        }

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message) => Selection = message.Item;
        void IHandle<IDocumentContext<GlyphEngine>>.Handle(IDocumentContext<GlyphEngine> message) => Engine = message.Context;
    }
}