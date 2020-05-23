using System.Collections.Generic;

namespace Calame.DataModelViewer.Base
{
    public abstract class EditorSourceBase<T> : IEditorSource
    {
        public string DisplayName { get; }
        public IEnumerable<string> FileExtensions { get; }

        protected EditorSourceBase(string displayName, IEnumerable<string> extensions)
        {
            DisplayName = displayName;
            FileExtensions = extensions;
        }

        protected EditorSourceBase(string displayName, params string[] extensions)
        {
            DisplayName = displayName;
            FileExtensions = extensions;
        }

        public abstract IEditor CreateEditor();
    }
}