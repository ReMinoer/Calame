namespace Calame
{
    public interface IDocumentContext<out T>
    {
        T Context { get; }
    }
}