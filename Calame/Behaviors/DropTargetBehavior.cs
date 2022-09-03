using System.Windows;
using Calame.Utils;
using Microsoft.Xaml.Behaviors;

namespace Calame.Behaviors
{
    public class DropTargetBehavior : Behavior<UIElement>
    {
        static public readonly DependencyProperty DropTargetProperty =
            DependencyProperty.Register(nameof(DropTarget), typeof(IDropTarget), typeof(DropTargetBehavior), new UIPropertyMetadata(null, null));

        public IDropTarget DropTarget
        {
            get => (IDropTarget)GetValue(DropTargetProperty);
            set => SetValue(DropTargetProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AllowDrop = true;

            AssociatedObject.DragEnter += OnDragEnter;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.DragLeave += OnDragLeave;
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.DragLeave -= OnDragLeave;
            AssociatedObject.DragOver -= OnDragOver;
            AssociatedObject.DragEnter -= OnDragEnter;

            AssociatedObject.AllowDrop = false;

            base.OnDetaching();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            DropTarget?.OnDragEnter(e);
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            DropTarget?.OnDragOver(e);
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            DropTarget?.OnDragLeave(e);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            DropTarget?.OnDrop(e);
        }
    }
}