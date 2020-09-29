namespace Calame
{
    public interface IDocumentContext
    {
        string WorkingDirectory { get; }
    }

    public interface IDocumentContext<out T> : IDocumentContext
    {
        T Context { get; }
    }
}