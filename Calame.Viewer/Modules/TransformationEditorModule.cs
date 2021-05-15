using System.ComponentModel.Composition;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Glyph.Composition;
using Glyph.Composition.Modelization;
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
        private GlyphObject _root;
        
        public TransformationEditorModule(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        protected override void HandleComponent(IGlyphComponent selection)
        {
            SceneNode sceneNode = selection.GetSceneNode();
            if (sceneNode == null)
                return;

            var transformationEditor = Model.EditorModeRoot.Add<MultiModeTransformationEditor>(beforeAdding: Model.NotSelectableComponents.Add);
            transformationEditor.EditedObject = new MultiModeTransformationController(sceneNode);
            transformationEditor.RaycastClient = Model.Client;

            _root = transformationEditor;
        }

        protected override void HandleData(IGlyphData selection)
        {
            var controller = new TransformableDataController(selection);
            if (controller.Anchor == null)
                return;

            var transformationEditor = Model.EditorModeRoot.Add<TransformationEditor>(beforeAdding: Model.NotSelectableComponents.Add);
            transformationEditor.EditedObject = controller;
            transformationEditor.RaycastClient = Model.Client;

            _root = transformationEditor;
        }

        protected override void ReleaseComponent(IGlyphComponent selection)
        {
            if (_root == null)
                return;
            
            Model.EditorModeRoot.RemoveAndDispose(_root);
            _root = null;
        }
    }
}