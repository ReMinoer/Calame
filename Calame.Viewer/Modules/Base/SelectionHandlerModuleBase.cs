using System;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Core;

namespace Calame.Viewer.Modules.Base
{
    public abstract class SelectionHandlerModuleBase : ViewerModuleBase, IHandle<ISelection<IGlyphComponent>>
    {
        private readonly IEventAggregator _eventAggregator;
        protected IBoxedComponent Selection;

        protected SelectionHandlerModuleBase(IEventAggregator eventAggregator)
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

        protected abstract void HandleSelection();
        protected abstract void ReleaseSelection();

        private bool _handlingSelection;
        void IHandle<ISelection<IGlyphComponent>>.Handle(ISelection<IGlyphComponent> message)
        {
            if (_handlingSelection)
                return;

            _handlingSelection = true;

            try
            {
                if (Runner.Engine.FocusedClient != Model.Client)
                    return;

                var boxedComponent = message.Item as IBoxedComponent;
                if (boxedComponent == Selection)
                    return;

                if (Selection != null)
                    ReleaseSelection();

                Selection = boxedComponent;

                if (Selection != null)
                    HandleSelection();
            }
            finally
            {
                _handlingSelection = false;
            }
        }
    }
}