using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Gemini.Framework.ToolBars;
using Glyph;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Microsoft.Xna.Framework.Graphics;
using Niddle;

namespace Calame.DataModelViewer
{
    public interface IEditor : IDisposable
    {
        IGlyphData Data { get; }
        ToolBarDefinition ToolBarDefinition { get; }

        Task NewDataAsync();
        Task LoadDataAsync(Stream stream);
        Task SaveDataAsync(Stream stream);

        IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService);
        void RegisterDependencies(IDependencyRegistry registry);
        void PrepareEditor(GlyphEngine engine, GlyphObject editorRoot);

        void OnDragOver(DragEventArgs dragEventArgs);
        void OnDrop(DragEventArgs dragEventArgs);
    }

    public interface IEditor<out TData> : IEditor
        where TData : IGlyphData
    {
        new TData Data { get; }
    }
}