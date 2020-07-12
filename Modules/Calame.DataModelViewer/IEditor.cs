using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Niddle;

namespace Calame.DataModelViewer
{
    public interface IEditor : IDisposable
    {
        IGlyphData Data { get; }
        string ContentPath { get; }

        Task NewDataAsync();
        Task LoadDataAsync(Stream stream);
        Task SaveDataAsync(Stream stream);

        void RegisterDependencies(IDependencyRegistry registry);
        void PrepareEditor(GlyphEngine engine, GlyphObject editorRoot);
        void OnDragOver(DragEventArgs dragEventArgs);
        void OnDrop(DragEventArgs dragEventArgs);
    }
}