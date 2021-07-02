using Calame.Viewer;
using Fingear.Interactives;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.SceneViewer
{
    public interface ISessionContext : IDefaultCameraTarget
    {
        GlyphEngine Engine { get; }
        IView RootView { get; }
        GlyphObject UserRoot { get; }
        IInteractiveComposite<IInteractive> SessionInteractive { get; }
        new IBoxedComponent DefaultCameraTarget { get; set; }
    }
}