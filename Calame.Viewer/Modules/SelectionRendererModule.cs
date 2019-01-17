using System;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Tools;
using Glyph.Tools.ShapeRendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.Viewer.Modules
{
    public class SelectionRendererModule : ViewerModuleBase, IHandle<ISelection<IGlyphComponent>>
    {
        private readonly IEventAggregator _eventAggregator;

        private IBoxedComponent _selection;
        private AreaComponentRenderer _selectionRenderer;

        public SelectionRendererModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected override void ConnectRunner()
        {
            _eventAggregator.Subscribe(this);
        }

        protected override void DisconnectRunner()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private void AddRenderer()
        {
            _selectionRenderer = new AreaComponentRenderer(_selection, Runner.Engine.Injector.Resolve<Func<GraphicsDevice>>())
            {
                Name = "Selection Renderer",
                Color = Color.Purple * 0.5f,
                DrawPredicate = drawer => ((Drawer)drawer).CurrentView.Camera.Parent is FreeCamera
            };

            Model.EditorRoot.Add(_selectionRenderer);
        }

        private void RemoveRenderer()
        {
            Model.EditorRoot.Remove(_selectionRenderer);

            _selectionRenderer.Dispose();
            _selectionRenderer = null;
        }

        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message)
        {
            if (Runner.Engine.FocusedClient != Model.Client)
                return;

            IBoxedComponent boxedComponent = null;
            if (message.Item != null)
            {
                boxedComponent = message.Item as IBoxedComponent;
                if (boxedComponent == null)
                    return;
            }

            if (boxedComponent == _selection)
                return;

            if (_selection != null)
                RemoveRenderer();

            _selection = boxedComponent;

            if (_selection != null)
                AddRenderer();
        }
    }
}