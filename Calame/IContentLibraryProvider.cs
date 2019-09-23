using Glyph;

namespace Calame
{
    public interface IContentLibraryProvider
    {
        IContentLibrary Get(string rootPath);
        bool Remove(string rootPath);
    }
}