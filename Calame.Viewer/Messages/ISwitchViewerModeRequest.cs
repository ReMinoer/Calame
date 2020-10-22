namespace Calame.Viewer.Messages
{
    public interface ISwitchViewerModeRequest : IDocumentMessage
    {
        bool Match(IViewerInteractiveMode mode);
        ISwitchViewerModeSpread Promoted(IViewerInteractiveMode mode);
    }
}