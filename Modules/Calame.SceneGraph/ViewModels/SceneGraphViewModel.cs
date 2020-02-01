using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Calame.Icons;
using Calame.UserControls;
using Calame.Utils;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.SceneGraph.ViewModels
{
    [Export(typeof(SceneGraphViewModel))]
    public sealed class SceneGraphViewModel : CalameTool<IDocumentContext<GlyphEngine>>, IHandle<ISelectionSpread<IGlyphComponent>>, IHandle<ISelectionSpread<IGlyphData>>, ITreeContext
    {
        public override PaneLocation PreferredLocation => PaneLocation.Left;

        public IIconProvider IconProvider { get; }
        private readonly IIconDescriptor<IGlyphComponent> _iconDescriptor;

        private GlyphEngine _engine;
        private IDocumentContext<IComponentFilter> _filteringContext;
        private IGlyphComponent _selection;
        private SceneNode _selectionNode;

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

                var selectionRequest = new SelectionRequest<IGlyphComponent>(CurrentDocument, _selection);
                EventAggregator.PublishOnBackgroundThreadAsync(selectionRequest).Wait();
            }
        }

        public SceneNode SelectionNode
        {
            get => _selectionNode;
            set
            {
                if (!SetValue(ref _selectionNode, value))
                    return;

                _selection = _selectionNode?.Parent;
                NotifyOfPropertyChange(nameof(Selection));
                
                var selectionRequest = new SelectionRequest<IGlyphComponent>(CurrentDocument, _selection);
                EventAggregator.PublishOnBackgroundThreadAsync(selectionRequest).Wait();
            }
        }

        [ImportingConstructor]
        public SceneGraphViewModel(IShell shell, IEventAggregator eventAggregator, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(shell, eventAggregator)
        {
            DisplayName = "Scene Graph";
            
            IconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor<IGlyphComponent>();

            if (shell.ActiveItem is IDocumentContext<GlyphEngine> documentContext)
                Engine = documentContext.Context;
        }

        protected override Task OnDocumentActivated(IDocumentContext<GlyphEngine> activeDocument)
        {
            _selection = null;
            _selectionNode = null;

            Engine = activeDocument.Context;
            _filteringContext = activeDocument as IDocumentContext<IComponentFilter>;

            return Task.CompletedTask;
        }

        protected override Task OnDocumentsCleaned()
        {
            _selection = null;
            _selectionNode = null;

            Engine = null;
            _filteringContext = null;

            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphComponent>>.HandleAsync(ISelectionSpread<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item);
            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            HandleSelection(message.Item?.BindedObject);
            return Task.CompletedTask;
        }

        private void HandleSelection(IGlyphComponent component)
        {
            SetValue(ref _selection, component, nameof(Selection));
            SetValue(ref _selectionNode, component?.GetSceneNode(), nameof(SelectionNode));
        }
        
        ITreeViewItemModel ITreeContext.CreateTreeItemModel(object data)
        {
            var sceneNode = (ISceneNode)data;
            var glyphComponent = (IGlyphComponent)data;

            return new TreeViewItemModel<ISceneNode>(
                this,
                sceneNode,
                x => (x as IGlyphComponent)?.Parent.Name ?? x.ToString(),
                x => x.Children,
                _iconDescriptor.GetIcon((sceneNode as IGlyphComponent)?.Parent),
                nameof(IGlyphComponent.Name),
                nameof(SceneNode.Children),
                (INotifyPropertyChanged)glyphComponent.Parent)
            {
                IsEnabled = _filteringContext?.Context.Filter(glyphComponent) ?? true
            };
        }
        
        bool ITreeContext.BaseFilter(object data) => true;
    }
}