using System.Collections.Generic;

namespace Calame.DataModelViewer
{
    public interface IEditorSource
    {
        string DisplayName { get; }
        IEnumerable<string> FileExtensions { get; }
        IEditor CreateEditor();
    }
}