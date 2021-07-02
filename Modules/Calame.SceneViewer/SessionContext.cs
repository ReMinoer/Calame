using Calame.Viewer.ViewModels;
using Fingear.Interactives;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.SceneViewer
{
    public class SessionContext : ISessionContext
    {
        public GlyphEngine Engine { get; }
        public IView RootView { get; }
        public GlyphObject UserRoot { get; }
        public IInteractiveComposite<IInteractive> SessionInteractive { get; }
        public IBoxedComponent DefaultCameraTarget { get; set; }

        public SessionContext(GlyphEngine engine)
        {
            Engine = engine;
            RootView = engine.RootView;
            UserRoot = engine.Root;
            SessionInteractive = engine.InteractionManager.Root;
            DefaultCameraTarget = UserRoot;
        }

        public SessionContext(ViewerViewModel viewerViewModel, IView rootView, IInteractiveComposite<IInteractive> sessionInteractive)
        {
            Engine = viewerViewModel.Runner.Engine;
            RootView = rootView;
            UserRoot = viewerViewModel.UserRoot;
            SessionInteractive = sessionInteractive;
            DefaultCameraTarget = UserRoot;
        }

        public SessionContext(GlyphEngine engine, IView rootView, GlyphObject userRoot, IInteractiveComposite<IInteractive> sessionInteractive, IBoxedComponent defaultCameraTarget)
        {
            Engine = engine;
            RootView = rootView;
            UserRoot = userRoot;
            SessionInteractive = sessionInteractive;
            DefaultCameraTarget = defaultCameraTarget;
        }

    }
}