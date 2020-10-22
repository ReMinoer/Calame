using System;

namespace Calame.Viewer.Messages
{
    public class SwitchViewerModeRequest : ISwitchViewerModeRequest
    {
        public IDocumentContext DocumentContext { get; }
        public IViewerInteractiveMode Mode { get; }

        public SwitchViewerModeRequest(IDocumentContext documentContext, IViewerInteractiveMode mode)
        {
            DocumentContext = documentContext;
            Mode = mode;
        }

        public bool Match(IViewerInteractiveMode mode) => Mode == mode;
        public ISwitchViewerModeSpread Promoted(IViewerInteractiveMode mode) => new SwitchViewerModeSpread(DocumentContext, mode);
    }

    public class SwitchViewerModeRequest<TMode> : ISwitchViewerModeRequest
        where TMode : IViewerInteractiveMode
    {
        public IDocumentContext DocumentContext { get; }
        public Type ModeType { get; } = typeof(TMode);

        public SwitchViewerModeRequest(IDocumentContext documentContext)
        {
            DocumentContext = documentContext;
        }

        public bool Match(IViewerInteractiveMode mode) => ModeType.IsInstanceOfType(mode);
        public ISwitchViewerModeSpread Promoted(IViewerInteractiveMode mode) => new SwitchViewerModeSpread(DocumentContext, mode);
    }
}