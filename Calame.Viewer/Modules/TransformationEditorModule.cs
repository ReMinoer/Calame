﻿using System;
using System.ComponentModel.Composition;
using Calame.DocumentContexts;
using Calame.Viewer.Modules.Base;
using Calame.Viewer.ViewModels;
using Caliburn.Micro;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Graphics.Renderer;
using Glyph.Tools.Base;
using Glyph.Tools.Snapping;
using Glyph.Tools.Transforming;
using Glyph.Tools.UndoRedo;
using Microsoft.Xna.Framework;
using Niddle;

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
        public IViewerModule CreateInstance(IDocumentContext documentContext)
        {
            return new TransformationEditorModule(
                _eventAggregator,
                documentContext.TryGetContext<IUndoRedoContext>(),
                documentContext as IViewerDocument);
        }
    }
    
    public class TransformationEditorModule : SelectionHandlerModuleBase
    {
        private readonly IUndoRedoContext _undoRedoContext;
        private readonly IViewerDocument _viewerDocument;

        private GlyphObject _root;
        private BoxedComponentsSnapping _boxedComponentsSnapping;
        private GridComponentsSnapping _gridComponentsSnapping;
        private InputModifiers _inputModifiers;
        private GlyphObject _markOwner;
        private GlyphObject _horizontalMark;
        private GlyphObject _verticalMark;
        private SceneNode _horizontalMarkSceneNode;
        private SceneNode _verticalMarkSceneNode;
        private IHandle _handle;

        public TransformationEditorModule(IEventAggregator eventAggregator, IUndoRedoContext undoRedoContext, IViewerDocument viewerDocument)
            : base(eventAggregator)
        {
            _undoRedoContext = undoRedoContext;
            _viewerDocument = viewerDocument;
        }

        protected override void ConnectRunner()
        {
            base.ConnectRunner();

            _root = Model.EditorModeRoot.Add<GlyphObject>();

            if (_undoRedoContext?.UndoRedoStack != null)
                _root.Resolver.Local.Registry.Add(Dependency.OnType<IUndoRedoStack>().Using(_undoRedoContext.UndoRedoStack));

            _boxedComponentsSnapping = new BoxedComponentsSnapping(Model.UserRoot);
            _gridComponentsSnapping = new GridComponentsSnapping(Model.UserRoot);

            _inputModifiers = _root.Add<InputModifiers>();

            _markOwner = _root.Add<GlyphObject>();
            _markOwner.Visible = false;

            _horizontalMark = _markOwner.Add<GlyphObject>();
            _horizontalMark.Visible = false;
            _horizontalMarkSceneNode = _horizontalMark.Add<SceneNode>();
            _horizontalMark.Add<MeshRenderer>().MeshProviders.Add(new LineMesh(Color.Yellow, -Vector2.UnitY * float.MaxValue, Vector2.Zero, Vector2.UnitY * float.MaxValue));

            _verticalMark = _markOwner.Add<GlyphObject>();
            _verticalMark.Visible = false;
            _verticalMarkSceneNode = _verticalMark.Add<SceneNode>();
            _verticalMark.Add<MeshRenderer>().MeshProviders.Add(new LineMesh(Color.Yellow, -Vector2.UnitX * float.MaxValue, Vector2.Zero, Vector2.UnitX * float.MaxValue));
        }

        protected override void DisconnectRunner()
        {
            _boxedComponentsSnapping.Dispose();
            _gridComponentsSnapping.Dispose();
            Model.EditorModeRoot.RemoveAndDispose(_root);

            _handle = null;
            _verticalMarkSceneNode = null;
            _horizontalMarkSceneNode = null;
            _verticalMark = null;
            _horizontalMark = null;
            _inputModifiers = null;
            _gridComponentsSnapping = null;
            _boxedComponentsSnapping = null;
            _root = null;

            base.DisconnectRunner();
        }

        protected override void HandleComponent(IGlyphComponent selection)
        {
            SceneNode sceneNode = selection.GetSceneNode();
            if (sceneNode == null)
                return;

            var transformationEditor = _root.Add<MultiModeTransformationEditor>(beforeAdding: Model.NotSelectableComponents.Add);
            transformationEditor.EditedObject = new MultiModeTransformationController(sceneNode);
            transformationEditor.RaycastClient = Model.Client;
            transformationEditor.Revaluation = x => Snap(selection, sceneNode.Position, x);

            _handle = transformationEditor;
            _handle.Dragging += OnGrabbed;
            _handle.Released += OnReleased;
            _handle.Cancelled += OnReleased;
        }

        protected override void HandleData(IGlyphData selection)
        {
            if (selection is IRectangleController rectangleController)
            {
                var anchoredController = rectangleController as IAnchoredController;
                var anchor = anchoredController?.Anchor ?? selection.BindedObject.GetSceneNode();
                var anchoredRectangleController = new AnchoredRectangleController(rectangleController, anchor);

                var rectangleEditor = _root.Add<RectangleEditor>(beforeAdding: Model.NotSelectableComponents.Add);
                rectangleEditor.EditedObject = anchoredRectangleController;
                rectangleEditor.ScaleAnchorNode = _viewerDocument?.Viewer.Runner.Engine.Root.GetSceneNode();
                rectangleEditor.RaycastClient = Model.Client;
                //rectangleEditor.Revaluation = x => Snap(selection.BindedObject, anchoredRectangleController.Rectangle.Position, x);

                _handle = rectangleEditor;
            }
            else
            {
                var controller = new TransformableDataController(selection);
                if (controller.Anchor == null)
                    return;

                var transformationEditor = _root.Add<TransformationEditor>(beforeAdding: Model.NotSelectableComponents.Add);
                transformationEditor.EditedObject = controller;
                transformationEditor.ScaleAnchorNode = _viewerDocument?.Viewer.Runner.Engine.Root.GetSceneNode();
                transformationEditor.RaycastClient = Model.Client;
                transformationEditor.Revaluation = x => Snap(selection.BindedObject, controller.PositionController.Position, x);

                _handle = transformationEditor;
            }

            _handle.Grabbed += OnGrabbed;
            _handle.Released += OnReleased;
            _handle.Cancelled += OnReleased;
        }

        private void OnGrabbed(object sender, EventArgs e)
        {
            _markOwner.Visible = true;
            _horizontalMark.Visible = false;
            _verticalMark.Visible = false;
        }

        private void OnReleased(object sender, EventArgs e)
        {
            _markOwner.Visible = false;
            _horizontalMark.Visible = false;
            _verticalMark.Visible = false;
        }

        private Vector2 Snap(IGlyphComponent component, Vector2 oldPosition, Vector2 newPosition)
        {
            Vector2 snappedPosition;
            float? horizontalSnap;
            float? verticalSnap;

            if (_inputModifiers.CtrlPressed)
            {
                snappedPosition = _boxedComponentsSnapping.Snap(component, oldPosition, newPosition, out horizontalSnap, out verticalSnap);
            }
            else if (_inputModifiers.AltPressed)
            {
                snappedPosition = _gridComponentsSnapping.Snap(component, oldPosition, newPosition, out horizontalSnap, out verticalSnap);
            }
            else
            {
                snappedPosition = newPosition;
                horizontalSnap = null;
                verticalSnap = null;
            }

            _horizontalMark.Visible = horizontalSnap.HasValue;
            if (horizontalSnap.HasValue)
                _horizontalMarkSceneNode.Position = _horizontalMarkSceneNode.Position.SetX(horizontalSnap.Value);

            _verticalMark.Visible = verticalSnap.HasValue;
            if (verticalSnap.HasValue)
                _verticalMarkSceneNode.Position = _verticalMarkSceneNode.Position.SetY(verticalSnap.Value);

            return snappedPosition;
        }

        protected override void ReleaseComponent(IGlyphComponent selection)
        {
            if (_handle == null)
                return;

            _handle.Cancelled -= OnReleased;
            _handle.Released -= OnReleased;
            _handle.Grabbed -= OnGrabbed;

            _root.RemoveAndDispose(_handle);
            _handle = null;
        }
    }
}