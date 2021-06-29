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
                    DisconnectViewer();

                _model = value;

                if (_model != null)
                    ConnectViewer();
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

        protected abstract void ConnectViewer();
        protected abstract void DisconnectViewer();
        protected abstract void ConnectRunner();
        protected abstract void DisconnectRunner();
        public abstract void Activate();
        public abstract void Deactivate();

        public virtual void Dispose()
        {
            if (Runner != null)
                Disconnect();
        }
    }
}