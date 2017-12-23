namespace Calame
{
    static public class DocumentContext
    {
        static public DocumentContext<T> New<T>(T context)
        {
            return new DocumentContext<T>(context);
        }
    }

    public class DocumentContext<T> : IDocumentContext<T>
    {
        public T Context { get; set; }

        public DocumentContext(T context)
        {
            Context = context;
        }
    }
}