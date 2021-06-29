using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.Viewer.Modules.Base
{
    public abstract class SelectionHandlerModuleBase : ViewerModuleBase, IHandle<ISelectionSpread<IGlyphComponent>>, IHandle<ISelectionSpread<IGlyphData>>
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _handlingSelection;
        private IGlyphComponent _componentSelection;
        private IGlyphData _dataSelection;

        protected SelectionHandlerModuleBase(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected override void ConnectViewer() {}
        protected override void DisconnectViewer() { }
        public override void Activate() { }
        public override void Deactivate() { }

        protected override void ConnectRunner()
        {
            _eventAggregator.SubscribeOnUI(this);
        }

        protected override void DisconnectRunner()
        {
            _eventAggregator.Unsubscribe(this);
        }

        protected abstract void HandleComponent(IGlyphComponent selection);
        protected abstract void ReleaseComponent(IGlyphComponent selection);

        protected virtual void HandleData(IGlyphData selection) => HandleComponent(selection?.BindedObject);
        protected virtual void ReleaseData(IGlyphData selection) => ReleaseComponent(selection?.BindedObject);

        Task IHandle<ISelectionSpread<IGlyphComponent>>.HandleAsync(ISelectionSpread<IGlyphComponent> message, CancellationToken cancellationToken)
        {
            HandleComponentSelection(message.Item);
            return Task.CompletedTask;
        }

        Task IHandle<ISelectionSpread<IGlyphData>>.HandleAsync(ISelectionSpread<IGlyphData> message, CancellationToken cancellationToken)
        {
            HandleDataSelection(message.Item);
            return Task.CompletedTask;
        }

        private void HandleComponentSelection(IGlyphComponent component)
        {
            if (_handlingSelection)
                return;

            _handlingSelection = true;

            try
            {
                if (Runner.Engine.FocusedClient != Model.Client)
                    return;
                if (_componentSelection == component)
                    return;

                Release();
                _componentSelection = component;

                if (_componentSelection != null)
                    HandleComponent(_componentSelection);
            }
            finally
            {
                _handlingSelection = false;
            }
        }

        private void HandleDataSelection(IGlyphData data)
        {
            if (_handlingSelection)
                return;

            _handlingSelection = true;

            try
            {
                if (Runner.Engine.FocusedClient != Model.Client)
                    return;
                if (_dataSelection == data)
                    return;

                Release();
                _dataSelection = data;

                if (_dataSelection != null)
                    HandleData(_dataSelection);
            }
            finally
            {
                _handlingSelection = false;
            }
        }

        private void Release()
        {
            if (_componentSelection != null)
                ReleaseComponent(_componentSelection);
            if (_dataSelection != null)
                ReleaseData(_dataSelection);
        }
    }
}