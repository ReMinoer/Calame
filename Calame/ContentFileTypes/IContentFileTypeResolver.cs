using System;
using System.Collections.Generic;
using Glyph.IO;

namespace Calame.ContentFileTypes
{
    public interface IContentFileTypeResolver
    {
        IEnumerable<FileType> GetFileTypes(Type contentType);
    }
}