using System;
using System.Windows.Controls;
using GraphShape.Controls;

namespace Calame.ViewGraph.Utils
{
    public class DirectTransition : ITransition
    {
        public void Run(IAnimationContext context, Control control, TimeSpan duration, Action<Control> endAction)
        {
            endAction?.Invoke(control);
        }
    }
}