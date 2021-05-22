using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Gemini.Framework.ToolBars;
using Glyph;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Glyph.IO;
using Glyph.Scheduling;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Niddle;

namespace Calame.DataModelViewer.Base
{
    public abstract class ViewerEditorBase<T> : IEditor<T>
        where T : IGlyphCreator
    {
        protected abstract ISaveLoadFormat<T> SaveLoadFormat { get; }

        public T Data { get; private set; }
        IGlyphData IEditor.Data => Data;

        public ToolBarDefinition ToolBarDefinition { get; set; }

        public async Task NewDataAsync()
        {
            Data = await NewAsync();
        }

        public async Task LoadDataAsync(Stream stream)
        {
            Data = await LoadAsync(stream);
        }
        
        public Task SaveDataAsync(Stream stream)
        {
            return SaveAsync(Data, stream);
        }

        public virtual Type RunCommandDefinitionType => null;

        protected abstract Task<T> NewAsync();
        protected virtual Task<T> LoadAsync(Stream stream) => Task.Run(() => SaveLoadFormat.Load(stream));
        protected virtual Task SaveAsync(T data, Stream stream) => Task.Run(() => SaveLoadFormat.Save(Data, stream));

        public abstract IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger);
        public abstract void RegisterDependencies(IDependencyRegistry registry);

        public virtual void PrepareEditor(GlyphEngine engine, GlyphObject editorRoot)
        {
            var pixel = new Texture2D(engine.Resolver.Resolve<Func<GraphicsDevice>>()(), 1, 1);
            pixel.SetData(new[] { Color.White });

            RenderScheduler renderScheduler = engine.RootView.RenderScheduler;

            renderScheduler.Plan(drawer => drawer.SpriteBatchStack.Current.Draw(pixel, drawer.DisplayedRectangle.BoundingBox.ToIntegers(), Color.CornflowerBlue))
                .Before(renderScheduler.RenderViewTask);

            editorRoot.Add<SceneNode>();

            Data.Instantiate();
            editorRoot.Add(Data.BindedObject);
        }

        public virtual void OnDragOver(DragEventArgs dragEventArgs)
        {
            dragEventArgs.Effects = DragDropEffects.None;
        }

        public virtual void OnDrop(DragEventArgs dragEventArgs)
        {
        }

        public void Dispose()
        {
            Data?.Dispose();
        }
    }
}