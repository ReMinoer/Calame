using System;
using System.Collections.Generic;

namespace Calame.DataModelViewer
{
    public interface IEditorSource
    {
        string DisplayName { get; }
        Type DataType { get; }
        IEnumerable<string> FileExtensions { get; }
        IEditor CreateEditor();
    }
}