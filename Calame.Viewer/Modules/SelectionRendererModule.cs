using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Meshes;
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

            var lineMesh = new LineMesh(Color.Purple);
            _root.Add<MeshesComponent>().Meshes.Add(lineMesh);
            var meshRenderer = _root.Add<MeshRenderer>();

            meshRenderer.DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera;

            _root.Schedulers.Update.Plan(_ =>
            {
                TopLeftRectangle rect = boxedSelection.Area.BoundingBox;
                Vector2[] vertices = { rect.Position, rect.P1, rect.P3, rect.P2, rect.Position };
                lineMesh.Points = vertices;
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