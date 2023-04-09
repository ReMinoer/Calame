using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Calame.Utils;
using Microsoft.Xaml.Behaviors;

namespace Calame.Behaviors
{
    public class DragSourceBehavior : Behavior<UIElement>
    {
        static public readonly DependencyProperty DragSourceProperty =
            DependencyProperty.Register(nameof(DragSource), typeof(IDragSource), typeof(DragSourceBehavior), new UIPropertyMetadata(null, null));

        public IDragSource DragSource
        {
            get => (IDragSource)GetValue(DragSourceProperty);
            set => SetValue(DragSourceProperty, value);
        }

        private Point? _startMousePosition;
        private UIElement _rootUiElement;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            base.OnDetaching();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WatchDragMove(e);
        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => UnwatchDragMove();

        private void WatchDragMove(MouseEventArgs e)
        {
            _startMousePosition = e.GetPosition(null);

            _rootUiElement = GetRootUiElement(AssociatedObject);
            _rootUiElement.MouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            _rootUiElement.PreviewMouseMove += OnPreviewMouseMove;
        }

        private void UnwatchDragMove()
        {
            if (_rootUiElement is null)
                return;
            
            _rootUiElement.PreviewMouseMove -= OnPreviewMouseMove;
            _rootUiElement.MouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            _rootUiElement = null;

            _startMousePosition = null;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_startMousePosition is null)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                // Unwatch in case mouse was released out of root ui element.
                UnwatchDragMove();
                return;
            }

            Point mousePosition = e.GetPosition(null);
            Vector diff = mousePosition - _startMousePosition.Value;

            if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance
                && Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            DraggedData draggedData = DragSource?.GetDraggedData();
            if (draggedData is null)
                return;

            UnwatchDragMove();
            DragDrop.DoDragDrop(AssociatedObject, draggedData.Data, draggedData.Effect);
        }

        static private UIElement GetRootUiElement(UIElement uiElement)
        {
            DependencyObject dependencyObject = uiElement;
            while (true)
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
                if (dependencyObject is null)
                    return uiElement;

                if (dependencyObject is UIElement element)
                    uiElement = element;
            }
        }
    }
}