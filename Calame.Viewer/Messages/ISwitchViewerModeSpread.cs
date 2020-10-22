namespace Calame.Viewer.Messages
{
    public interface ISwitchViewerModeSpread : IDocumentMessage
    {
        IViewerInteractiveMode Mode { get; }
    }
}