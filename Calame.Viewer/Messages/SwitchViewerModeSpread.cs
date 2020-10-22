namespace Calame.Viewer.Messages
{
    public class SwitchViewerModeSpread : ISwitchViewerModeSpread
    {
        public IDocumentContext DocumentContext { get; }
        public IViewerInteractiveMode Mode { get; }

        public SwitchViewerModeSpread(IDocumentContext documentContext, IViewerInteractiveMode mode)
        {
            DocumentContext = documentContext;
            Mode = mode;
        }
    }
}