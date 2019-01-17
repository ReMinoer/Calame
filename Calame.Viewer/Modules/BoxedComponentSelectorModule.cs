using System;
using Calame.Viewer.Modules.Base;
using Caliburn.Micro;
using Fingear.Controls;
using Fingear.Controls.Containers;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Tools;

namespace Calame.Viewer.Modules
{
    public class BoxedComponentSelectorModule : ViewerModuleBase
    {
        private readonly IEventAggregator _eventAggregator;
        private ShapedObjectSelector _shapedObjectSelector;
        private IBoxedComponent _selectedComponent;

        public IBoxedComponent SelectedComponent
        {
            get => _selectedComponent;
            private set
            {
                if (this.SetValue(ref _selectedComponent, value))
                    SelectionChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<IBoxedComponent> SelectionChanged;

        public BoxedComponentSelectorModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        
        protected override void ConnectRunner()
        {
            GlyphEngine engine = Model.Runner.Engine;

            _shapedObjectSelector = Model.EditorRoot.Add<ShapedObjectSelector>();

            _shapedObjectSelector.HandleInputs = true;
            _shapedObjectSelector.Control = new HybridControl<System.Numerics.Vector2>("Pointer")
            {
                TriggerControl = new Control(InputSystem.Instance.Mouse[MouseButton.Left]),
                ValueControl = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, engine.RootView, new ReadOnlySceneNodeDelegate(Model.EditorCamera.GetSceneNode), engine.ProjectionManager)
            };

            _shapedObjectSelector.SelectionChanged += OnShapedObjectSelectorSelectionChanged;
        }

        protected override void DisconnectRunner()
        {
            _shapedObjectSelector.SelectionChanged -= OnShapedObjectSelectorSelectionChanged;

            Model.EditorRoot.Remove(_shapedObjectSelector);

            _shapedObjectSelector.Dispose();
            _shapedObjectSelector = null;
        }

        private void OnShapedObjectSelectorSelectionChanged(object sender, IBoxedComponent boxedComponent)
        {
            if (Model.Runner.Engine.FocusedClient != Model.Client)
                return;

            SelectedComponent = boxedComponent;
            _eventAggregator.PublishOnUIThread(Selection.Of(SelectedComponent));
        }
    }
}