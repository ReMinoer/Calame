﻿using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
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

        protected override void HandleSelection()
        {
            _root = Model.EditorModeRoot.Add<GlyphObject>(Model.ComponentsFilter.ExcludedRoots.Add);
            _root.Add<SceneNode>();

            var primitiveComponent = _root.Add<PrimitiveComponent<LinePrimitive>>();
            var primitiveRenderer = _root.Add<PrimitiveRenderer>();
            primitiveRenderer.DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera;

            _root.Schedulers.Update.Plan(_ =>
            {
                TopLeftRectangle rect = Selection.Area.BoundingBox;
                Vector2[] vertices = { rect.Position, rect.P1, rect.P3, rect.P2, rect.Position };
                primitiveComponent.Primitive = new LinePrimitive(Color.Purple, vertices);
            });
        }

        protected override void ReleaseSelection()
        {
            Model.EditorModeRoot.RemoveAndDispose(_root);
            _root = null;
        }
    }
}