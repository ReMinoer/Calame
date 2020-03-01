using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Core;
using Glyph.Tools;
using Glyph.Tools.Transforming;

namespace Calame.Viewer.Modules
{
    [Export(typeof(IViewerModuleSource))]
    public class SceneNodeEditorModuleSource : IViewerModuleSource
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public SceneNodeEditorModuleSource(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool IsValidForDocument(IDocumentContext documentContext) => true;
        public IViewerModule CreateInstance() => new SceneNodeEditorModule(_eventAggregator);
    }
    
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

            _sceneNodeEditor = Model.EditorModeRoot.Add<SceneNodeEditor>(beforeAdding: Model.ComponentsFilter.ExcludedRoots.Add);
            _sceneNodeEditor.EditedObject = sceneNode;
            _sceneNodeEditor.RaycastClient = Model.Client;
        }

        protected override void ReleaseSelection()
        {
            if (_sceneNodeEditor == null)
                return;
            
            Model.EditorModeRoot.RemoveAndDispose(_sceneNodeEditor);

            _sceneNodeEditor = null;
        }
    }
}