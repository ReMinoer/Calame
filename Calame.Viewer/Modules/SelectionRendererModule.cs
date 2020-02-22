using System;
using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Graphics;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private AreaComponentRenderer _selectionRenderer;
        
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

            Model.ComponentsFilter.ExcludedRoots.Add(_selectionRenderer);
            
            Model.EditorModeRoot.Add(_selectionRenderer);
        }

        protected override void ReleaseSelection()
        {
            Model.EditorModeRoot.RemoveAndDispose(_selectionRenderer);

            _selectionRenderer.Dispose();
            _selectionRenderer = null;
        }
    }
}