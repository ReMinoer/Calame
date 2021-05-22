using Glyph.Pipeline;

namespace Calame.DocumentContexts
{
    public interface IRawContentLibraryContext : IContentLibraryContext
    {
        IRawContentLibrary RawContentLibrary { get; }
    }
}