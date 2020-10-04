using System;
using System.Collections.Generic;

namespace Calame.DataModelViewer.Base
{
    public abstract class EditorSourceBase : IEditorSource
    {
        public string DisplayName { get; }
        public Type DataType { get; }
        public IEnumerable<string> FileExtensions { get; }

        protected EditorSourceBase(string displayName, Type dataType, IEnumerable<string> extensions)
        {
            DisplayName = displayName;
            DataType = dataType;
            FileExtensions = extensions;
        }

        protected EditorSourceBase(string displayName, Type dataType, params string[] extensions)
        {
            DisplayName = displayName;
            DataType = dataType;
            FileExtensions = extensions;
        }

        public abstract IEditor CreateEditor();
    }
}