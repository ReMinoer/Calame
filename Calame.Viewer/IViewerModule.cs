using System;
using Calame.Viewer.ViewModels;

namespace Calame.Viewer
{
    public interface IViewerModule : IDisposable
    {
        ViewerViewModel Model { set; }
        void Connect();
        void Disconnect();
    }
}