using System;

namespace Calame.Viewer
{
    public interface IViewerModule : IDisposable
    {
        void Connect(ViewerViewModel model);
        void Disconnect();
    }
}