using System;
using System.Collections.Generic;
using Calame.ContentFileTypes;
using Glyph.IO;
using Glyph.Pipeline;

namespace Calame.PropertyGrid.Utils
{
    public class PropertyGridContentFileTypeResolver : IContentFileTypeResolver
    {
        public IContentFileTypeResolver DefaultResolver { get; set; }
        public IRawContentLibrary RawContentLibrary { get; set; }

        public IEnumerable<FileType> GetFileTypes(Type contentType)
        {
            if (DefaultResolver != null)
            {
                foreach (FileType fileType in DefaultResolver.GetFileTypes(contentType))
                    yield return fileType;
            }

            if (RawContentLibrary != null)
            {
                foreach (string extension in RawContentLibrary.GetSupportedFileExtensions(contentType))
                    yield return new FileType(extension);
            }
        }
    }
}