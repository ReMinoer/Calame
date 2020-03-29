using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Core;
using Glyph.Tools.Transforming;

namespace Calame.Viewer.Modules
{
    [Export(typeof(IViewerModuleSource))]
    public class TransformationEditorModuleSource : IViewerModuleSource
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public TransformationEditorModuleSource(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool IsValidForDocument(IDocumentContext documentContext) => true;
        public IViewerModule CreateInstance() => new TransformationEditorModule(_eventAggregator);
    }
    
    public class TransformationEditorModule : SelectionHandlerModuleBase
    {
        private MultiModeTransformationEditor _transformationEditor;
        
        public TransformationEditorModule(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        protected override void HandleSelection()
        {
            SceneNode sceneNode = Selection.GetSceneNode();
            if (sceneNode == null)
                return;

            _transformationEditor = Model.EditorModeRoot.Add<MultiModeTransformationEditor>(beforeAdding: Model.ComponentsFilter.ExcludedRoots.Add);
            _transformationEditor.EditedObject = new MultiModeTransformationController(sceneNode);
            _transformationEditor.RaycastClient = Model.Client;
        }

        protected override void ReleaseSelection()
        {
            if (_transformationEditor == null)
                return;
            
            Model.EditorModeRoot.RemoveAndDispose(_transformationEditor);

            _transformationEditor = null;
        }
    }
}