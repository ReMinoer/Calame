using System;
using System.Collections.Generic;
using System.IO;
using Glyph;

namespace Calame
{
    public class ContentLibraryProvider : IContentLibraryProvider
    {
        static private readonly IContentLibrary NullContentLibrary = new UnusedContentLibrary();
        private readonly Dictionary<string, IContentLibrary> _contentLibraries = new Dictionary<string, IContentLibrary>(StringComparer.OrdinalIgnoreCase);

        public IContentLibrary Get(string path)
        {
            if (path == null)
                return NullContentLibrary;

            string fullPath = Path.GetFullPath(path);
            if (!_contentLibraries.TryGetValue(fullPath, out IContentLibrary contentManager))
                _contentLibraries.Add(fullPath, contentManager = new ContentLibrary(WpfGraphicsDeviceService.Instance, path));

            return contentManager;
        }

        public bool Remove(string path)
        {
            return _contentLibraries.Remove(Path.GetFullPath(path));
        }
    }
}