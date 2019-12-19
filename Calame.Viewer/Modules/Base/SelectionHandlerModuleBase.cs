using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;

namespace Calame.Viewer.Modules.Base
{
    public abstract class SelectionHandlerModuleBase : ViewerModuleBase, IHandle<ISelectionSpread<IGlyphComponent>>, IHandle<ISelectionSpread<IGlyphData>>
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _handlingSelection;
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

        void IHandle<ISelectionSpread<IGlyphComponent>>.Handle(ISelectionSpread<IGlyphComponent> message) => HandleSelection(message.Item);
        void IHandle<ISelectionSpread<IGlyphData>>.Handle(ISelectionSpread<IGlyphData> message) => HandleSelection(message.Item?.BindedObject);

        private void HandleSelection(IGlyphComponent component)
        {
            if (_handlingSelection)
                return;

            _handlingSelection = true;

            try
            {
                if (Runner.Engine.FocusedClient != Model.Client)
                    return;

                var boxedComponent = component as IBoxedComponent;
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