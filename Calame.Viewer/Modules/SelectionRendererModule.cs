using System;
using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Graphics;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.Viewer.Modules
{
    [Export(typeof(IViewerModule))]
    public class SelectionRendererModule : SelectionHandlerModuleBase, IHandle<ISelection<IGlyphComponent>>
    {
        private AreaComponentRenderer _selectionRenderer;
        
        [ImportingConstructor]
        public SelectionRendererModule(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        protected override void HandleSelection()
        {
            _selectionRenderer = new AreaComponentRenderer(Selection, Runner.Engine.Resolver.Resolve<Func<GraphicsDevice>>())
            {
                Name = "Selection Renderer",
                Color = Color.Purple * 0.5f,
                DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera
            };

            Model.EditorRoot.Add(_selectionRenderer);
        }

        protected override void ReleaseSelection()
        {
            Model.EditorRoot.Remove(_selectionRenderer);

            _selectionRenderer.Dispose();
            _selectionRenderer = null;
        }
    }
}