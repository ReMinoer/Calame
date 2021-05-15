using System;
using Caliburn.Micro;
using Glyph;

namespace Calame
{
    static public class DocumentContextExtension
    {
        static public T TryGetContext<T>(this IDocumentContext documentContext)
            where T : class
        {
            return (documentContext as IDocumentContext<T>)?.Context;
        }

        static public T GetContext<T>(this IDocumentContext documentContext)
            where T : class
        {
            return documentContext.TryGetContext<T>() ?? throw new InvalidCastException();
        }
    }
}