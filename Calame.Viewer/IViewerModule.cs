using System;

namespace Calame.Viewer
{
    public interface IViewerModule : IDisposable
    {
        bool IsValidForDocument(IDocumentContext documentContext);
        void Connect(ViewerViewModel model);
        void Disconnect();
    }
}