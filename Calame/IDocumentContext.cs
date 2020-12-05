using Gemini.Framework;

namespace Calame
{
    public interface IDocumentContext
    {
        IDocument Document { get; }
        string WorkingDirectory { get; }
    }

    public interface IDocumentContext<out T> : IDocumentContext
    {
        T Context { get; }
    }
}