using System;
using System.Windows.Media;
using Caliburn.Micro;
using Glyph;
using Glyph.Core;

namespace Calame.ViewGraph.Graph
{
    public class ViewGraphVertex : PropertyChangedBase, IDisposable
    {
        private readonly IView _view;
        private readonly ISceneNode _rootNode;

        private object _data;
        public object Data
        {
            get => _data;
            private set => Set(ref _data, value);
        }

        private Brush _color;
        public Brush Color
        {
            get => _color;
            private set => Set(ref _color, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (Set(ref _isSelected, value) && _isSelected)
                    Selected?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Selected;
        public event EventHandler Dirtied;

        public ViewGraphVertex(IView view)
        {
            Data = view;
            _view = view;
            _view.CameraChanged += OnCameraChanged;
            
            Color = Brushes.Purple;
        }

        public ViewGraphVertex(ISceneNode sceneNode, ISceneNode rootNode)
        {
            Data = sceneNode;
            _rootNode = rootNode;
            _rootNode.ParentNodeChanged += OnRootNodeChanged;
            
            Color = Brushes.Blue;
        }

        private void OnCameraChanged(object sender, ICamera e)
        {
            Dirtied?.Invoke(this, EventArgs.Empty);
        }

        private void OnRootNodeChanged(object sender, ISceneNode e)
        {
            Dirtied?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_view != null)
                _view.CameraChanged -= OnCameraChanged;
            if (_rootNode != null)
                _rootNode.ParentNodeChanged -= OnRootNodeChanged;
        }
    }
}