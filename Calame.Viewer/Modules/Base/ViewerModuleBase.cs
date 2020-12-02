using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Glyph.WpfInterop;

namespace Calame.Viewer.Modules.Base
{
    public abstract class ViewerModuleBase : PropertyChangedBase, IViewerModule
    {
        private ViewerViewModel _model;
        public ViewerViewModel Model
        {
            get => _model;
            set
            {
                if (_model == value)
                    return;

                if (_model != null)
                    DisconnectModel();

                _model = value;

                if (_model != null)
                    ConnectModel();
            }
        }

        protected GlyphWpfRunner Runner { get; private set; }

        public void Connect()
        {
            Runner = Model?.Runner;
            
            if (Runner != null)
                ConnectRunner();
        }

        public void Disconnect()
        {
            if (Runner != null)
                DisconnectRunner();

            Runner = null;
        }

        protected abstract void ConnectModel();
        protected abstract void DisconnectModel();
        protected abstract void ConnectRunner();
        protected abstract void DisconnectRunner();

        public virtual void Dispose()
        {
            if (Runner != null)
                Disconnect();
        }
    }
}