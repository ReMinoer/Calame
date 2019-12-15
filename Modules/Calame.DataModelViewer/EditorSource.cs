using System.Collections.Generic;
using Glyph.IO;

namespace Calame.DataModelViewer
{
    public abstract class EditorSource<T> : IEditorSource
    {
        protected abstract ISaveLoadFormat<T> SaveLoadFormat { get; }
        public abstract IEditor CreateInstance();

        public string DisplayName => SaveLoadFormat.FileType.DisplayName;
        public IEnumerable<string> FileExtensions => SaveLoadFormat.FileType.Extensions;
    }
}