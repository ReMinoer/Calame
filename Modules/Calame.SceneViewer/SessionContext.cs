using Calame.Viewer;
using Fingear;
using Fingear.Interactives;
using Glyph.Core;
using Glyph.Engine;

namespace Calame.SceneViewer
{
    public class SessionContext : ISessionContext
    {
        public GlyphEngine Engine { get; }
        public IView RootView { get; }
        public GlyphObject EditorRoot { get; }
        public IInteractiveComposite<IInteractive> SessionInteractive { get; }

        public SessionContext(GlyphEngine engine)
        {
            Engine = engine;
            RootView = engine.RootView;
            EditorRoot = engine.Root;
            SessionInteractive = engine.InteractionManager.Root;
        }

        public SessionContext(ViewerViewModel viewerViewModel, IView rootView, IInteractiveComposite<IInteractive> sessionInteractive)
        {
            Engine = viewerViewModel.Runner.Engine;
            RootView = rootView;
            EditorRoot = viewerViewModel.EditorRoot;
            SessionInteractive = sessionInteractive;
        }

        public SessionContext(GlyphEngine engine, IView rootView, GlyphObject editorRoot, IInteractiveComposite<IInteractive> sessionInteractive)
        {
            Engine = engine;
            RootView = rootView;
            EditorRoot = editorRoot;
            SessionInteractive = sessionInteractive;
        }
    }
}