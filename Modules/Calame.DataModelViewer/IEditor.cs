using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Gemini.Framework.ToolBars;
using Glyph;
using Glyph.Composition.Modelization;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.DataModelViewer
{
    public interface IEditor : IDisposable
    {
        IGlyphData Data { get; }

        ToolBarDefinition ToolBarDefinition { get; }
        Type RunCommandDefinitionType { get; }
        Type RunAlternativeCommandDefinitionType { get; }

        Task NewDataAsync();
        Task LoadDataAsync(Stream stream);
        Task SaveDataAsync(Stream stream);

        IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger);
        void PrepareEditor(IEditorContext editorContext);

        void OnDragOver(DragEventArgs dragEventArgs);
        void OnDrop(DragEventArgs dragEventArgs);
    }

    public interface IEditor<out TData> : IEditor
        where TData : IGlyphData
    {
        new TData Data { get; }
    }
}