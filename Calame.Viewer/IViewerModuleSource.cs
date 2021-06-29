namespace Calame.Viewer
{
    public interface IViewerModuleSource
    {
        bool IsValidForDocument(IDocumentContext documentContext);
        IViewerModule CreateInstance(IDocumentContext documentContext);
    }
}