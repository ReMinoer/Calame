using Caliburn.Micro;
using Glyph.WpfInterop;

namespace Calame.Viewer.Modules.Base
{
    public abstract class ViewerModuleBase : PropertyChangedBase, IViewerModule
    {
        protected ViewerViewModel Model { get; private set; }
        protected GlyphWpfRunner Runner { get; private set; }

        public virtual bool IsValidForDocument(IDocumentContext documentContext) => true;

        public void Connect(ViewerViewModel model)
        {
            Model = model;
            Runner = model.Runner;
            
            if (Runner != null)
                ConnectRunner();
        }

        public void Disconnect()
        {
            if (Runner != null)
                DisconnectRunner();

            Runner = null;
            Model = null;
        }

        protected abstract void ConnectRunner();
        protected abstract void DisconnectRunner();

        public virtual void Dispose()
        {
            if (Model != null)
                Disconnect();
        }
    }
}