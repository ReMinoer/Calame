using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Core;
using Glyph.Tools;

namespace Calame.Viewer.Modules
{
    public class SceneNodeEditorModule : SelectionHandlerModuleBase
    {
        private SceneNodeEditor _sceneNodeEditor;

        public SceneNodeEditorModule(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        protected override void HandleSelection()
        {
            SceneNode sceneNode = Selection.GetSceneNode();
            if (sceneNode == null)
                return;

            _sceneNodeEditor = Model.EditorRoot.Add<SceneNodeEditor>();
            _sceneNodeEditor.EditedObject = sceneNode;
        }

        protected override void ReleaseSelection()
        {
            if (_sceneNodeEditor == null)
                return;

            Model.EditorRoot.RemoveAndDispose(_sceneNodeEditor);
            _sceneNodeEditor = null;
        }
    }
}