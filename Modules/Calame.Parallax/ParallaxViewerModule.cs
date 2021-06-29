using System.ComponentModel.Composition;
using Calame.DocumentContexts;
using Calame.Viewer;
using Calame.Viewer.Modules.Base;
using Diese.Collections;
using Glyph.Core;
using Glyph.Tools.Parallax;

namespace Calame.Parallax
{
    [Export(typeof(IViewerModuleSource))]
    public class ParallaxViewerModuleSource : IViewerModuleSource
    {
        public bool IsValidForDocument(IDocumentContext documentContext) => documentContext is IDocumentContext<IRootDataContext>;
        public IViewerModule CreateInstance(IDocumentContext documentContext) => new ParallaxViewerModule(((IDocumentContext<IRootDataContext>)documentContext).Context);
    }

    public class ParallaxViewerModule : ViewerModuleBase
    {
        private readonly IRootDataContext _rootDataContext;
        private GlyphObject _root;

        public ParallaxViewerModule(IRootDataContext rootDataContext)
        {
            _rootDataContext = rootDataContext;
        }

        protected override void ConnectViewer() {}
        protected override void DisconnectViewer() { }
        protected override void ConnectRunner() { }
        protected override void DisconnectRunner() { }

        public override void Activate()
        {
            var parallaxControllerSettings = _rootDataContext.RootData.FirstOfTypeOrDefault<IParallaxManipulatorSettings>();
            if (parallaxControllerSettings == null)
                return;

            _root = Model.EditorModeRoot.Add<GlyphObject>(beforeAdding: Model.NotSelectableComponents.Add);
            _root.Name = "Parallax Controller";

            var parallaxController = _root.Add<ParallaxManipulator>();
            parallaxController.Settings = parallaxControllerSettings;
        }

        public override void Deactivate()
        {
            Model.EditorModeRoot.RemoveAndDispose(_root);
            _root = null;
        }
    }
}