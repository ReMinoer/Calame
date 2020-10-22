using System;
using Calame.Viewer.ViewModels;

namespace Calame.Viewer
{
    public interface IViewerModule : IDisposable
    {
        void Connect(ViewerViewModel model);
        void Disconnect();
    }
}