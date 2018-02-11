using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Calame
{
    // Source: http://badecho.com/2012/07/adding-interface-support-to-datatemplates/
    public sealed class InterfaceTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null || !(container is FrameworkElement containerElement))
                return base.SelectTemplate(item, container);

            Type itemType = item.GetType();

            IEnumerable<Type> dataTypes
                = Enumerable.Repeat(itemType, 1).Concat(itemType.GetInterfaces());

            DataTemplate template
                = dataTypes.Select(t => new DataTemplateKey(t))
                           .Select(containerElement.TryFindResource)
                           .OfType<DataTemplate>()
                           .FirstOrDefault();

            return template ?? base.SelectTemplate(item, container);
        }
    }
}