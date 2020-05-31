using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
using Glyph.Math.Shapes;
using Glyph.Tools;
using Microsoft.Xna.Framework;

namespace Calame.Viewer.Modules
{
    [Export(typeof(IViewerModuleSource))]
    public class SelectionRendererModuleSource : IViewerModuleSource
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public SelectionRendererModuleSource(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool IsValidForDocument(IDocumentContext documentContext) => true;
        public IViewerModule CreateInstance() => new SelectionRendererModule(_eventAggregator);
    }
    
    public class SelectionRendererModule : SelectionHandlerModuleBase
    {
        private GlyphObject _root;

        public SelectionRendererModule(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        protected override void HandleComponent(IGlyphComponent selection)
        {
            if (!(selection is IBoxedComponent boxedSelection))
                return;

            _root = Model.EditorModeRoot.Add<GlyphObject>(Model.ComponentsFilter.ExcludedRoots.Add);
            _root.Add<SceneNode>();

            var linePrimitive = new LinePrimitive(Color.Purple);
            _root.Add<PrimitivesComponent>().Primitives.Add(linePrimitive);
            var primitiveRenderer = _root.Add<PrimitiveRenderer>();

            primitiveRenderer.DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera;

            _root.Schedulers.Update.Plan(_ =>
            {
                TopLeftRectangle rect = boxedSelection.Area.BoundingBox;
                Vector2[] vertices = { rect.Position, rect.P1, rect.P3, rect.P2, rect.Position };
                linePrimitive.Points = vertices;
            });
        }

        protected override void ReleaseComponent(IGlyphComponent selection)
        {
            if (_root != null)
            {
                Model.EditorModeRoot.RemoveAndDispose(_root);
                _root = null;
            }
        }
    }
}