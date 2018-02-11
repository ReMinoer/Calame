using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Calame.Utils
{
    [ContentProperty("TypeArgument")]
    public class EnumerableTypeExtension : MarkupExtension
    {
        public Type TypeArgument { get; set; }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return typeof(IEnumerable<>).MakeGenericType(TypeArgument);
        }
    }
}