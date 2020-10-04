using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Glyph.IO;

namespace Calame.ContentFileTypes
{
    [Export(typeof(IContentFileTypeResolver))]
    public class ContentFileExtensionsResolver : IContentFileTypeResolver
    {
        private readonly IContentFileTypeResolverModule[] _modules;

        [ImportingConstructor]
        public ContentFileExtensionsResolver([ImportMany] IContentFileTypeResolverModule[] modules)
        {
            _modules = modules;
        }

        public IEnumerable<FileType> GetFileTypes(Type contentType) => _modules.SelectMany(x => x.GetFileTypes(contentType));
    }
}