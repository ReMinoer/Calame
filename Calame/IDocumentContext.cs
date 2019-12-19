namespace Calame
{
    public interface IDocumentContext
    {
    }

    public interface IDocumentContext<out T> : IDocumentContext
    {
        T Context { get; }
    }
}