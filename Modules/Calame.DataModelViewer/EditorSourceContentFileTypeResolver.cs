using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Calame.ContentFileTypes;
using Glyph.IO;

namespace Calame.DataModelViewer
{
    [Export(typeof(IContentFileTypeResolverModule))]
    public class EditorSourceContentFileTypeResolver : IContentFileTypeResolverModule
    {
        private readonly IEditorSource[] _dataEditorProviders;

        [ImportingConstructor]
        public EditorSourceContentFileTypeResolver([ImportMany] IEditorSource[] dataEditorProviders)
        {
            _dataEditorProviders = dataEditorProviders;
        }

        public IEnumerable<FileType> GetFileTypes(Type contentType)
        {
            return _dataEditorProviders.Where(x => contentType.IsAssignableFrom(x.DataType)).Select(x => new FileType
            {
                DisplayName = $"{x.DisplayName} Files",
                Extensions = x.FileExtensions.ToArray()
            });
        }
    }
}