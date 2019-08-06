using System.ComponentModel;
using System.ComponentModel.Composition;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.SceneGraph.ViewModels
{
    [Export(typeof(SceneGraphViewModel))]
    public sealed class SceneGraphViewModel : HandleTool, IHandle<IDocumentContext<GlyphEngine>>, IHandle<ISelection<IGlyphComponent>>, ITreeContext
    {
        private GlyphEngine _engine;
        private IGlyphComponent _selection;
        private SceneNode _selectionNode;
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
                if (SetValue(ref _selection, value))
                {
                    _selectionNode = _selection?.GetSceneNode();
                    NotifyOfPropertyChange(nameof(SelectionNode));
                }
 
                EventAggregator.PublishOnUIThread(new Selection<IGlyphComponent>(_selection));
            }
        }

        public SceneNode SelectionNode
        {
            get => _selectionNode;
            set
            {
                if (SetValue(ref _selectionNode, value))
                {
                    _selection = _selectionNode?.Parent;
                    NotifyOfPropertyChange(nameof(Selection));
                }

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

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message)
        {
            SetValue(ref _selection, message.Item, nameof(Selection));
            SetValue(ref _selectionNode, message.Item?.GetSceneNode(), nameof(SelectionNode));
        }

        void IHandle<IDocumentContext<GlyphEngine>>.Handle(IDocumentContext<GlyphEngine> message) => Engine = message.Context;
        
        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data)
        {
            var sceneNode = (ISceneNode)data;
            var glyphComponent = (IGlyphComponent)data;

            return new TreeViewItemModel<ISceneNode>(
                this,
                sceneNode,
                x => (x as IGlyphComponent)?.Parent.Name ?? x.ToString(),
                x => x.Children,
                nameof(IGlyphComponent.Name),
                nameof(SceneNode.Children),
                (INotifyPropertyChanged)glyphComponent.Parent);
        }
        
        bool ITreeContext.BaseFilter(object data) => true;
    }
}