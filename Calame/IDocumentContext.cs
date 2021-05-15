using Gemini.Framework;

namespace Calame
{
    public interface IDocumentContext
    {
        IDocument Document { get; }
    }

    public interface IDocumentContext<out T> : IDocumentContext
    {
        T Context { get; }
    }
}