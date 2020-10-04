using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.IO;

namespace Calame.ContentFileTypes
{
    public abstract class FavoriteFileTypeResolverBase<TContentType> : IContentFileTypeResolverModule
    {
        protected abstract IEnumerable<FileType> FileTypes { get; }

        public IEnumerable<FileType> GetFileTypes(Type contentType)
        {
            if (contentType.IsAssignableFrom(typeof(TContentType)))
                return FileTypes;

            return Enumerable.Empty<FileType>();
        }
    }
}