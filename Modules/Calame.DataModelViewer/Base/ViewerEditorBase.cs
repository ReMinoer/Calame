using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Glyph;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Glyph.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Niddle;

namespace Calame.DataModelViewer.Base
{
    public abstract class ViewerEditorBase<T> : IEditor
        where T : IGlyphCreator
    {
        protected abstract ISaveLoadFormat<T> SaveLoadFormat { get; }
        protected T Creator { get; private set; }
        public abstract string ContentPath { get; }

        public IGlyphData Data => Creator;
        
        public async Task NewDataAsync()
        {
            Creator = await NewAsync();
        }

        public async Task LoadDataAsync(Stream stream)
        {
            Creator = await LoadAsync(stream);
        }
        
        public Task SaveDataAsync(Stream stream)
        {
            return SaveAsync(Creator, stream);
        }
        
        protected abstract Task<T> NewAsync();
        protected virtual Task<T> LoadAsync(Stream stream) => Task.Run(() => SaveLoadFormat.Load(stream));
        protected virtual Task SaveAsync(T data, Stream stream) => Task.Run(() => SaveLoadFormat.Save(Creator, stream));

        public virtual void RegisterDependencies(IDependencyRegistry registry)
        {
        }

        public virtual void PrepareEditor(GlyphEngine engine, GlyphObject editorRoot)
        {
            var pixel = new Texture2D(engine.Resolver.Resolve<Func<GraphicsDevice>>()(), 1, 1);
            pixel.SetData(new[] { Color.White });

            editorRoot.Schedulers.Draw.Plan(drawer =>
            {
                if (drawer.DrawPredicate(engine.RootView.Camera.GetSceneNode()))
                    drawer.SpriteBatchStack.Current.Draw(pixel, drawer.DisplayedRectangle.BoundingBox.ToIntegers(), Color.CornflowerBlue);
            }).AtStart();

            var dataRoot = editorRoot.Add<GlyphObject>();
            dataRoot.Name = "Data Root";
            dataRoot.Add<SceneNode>();

            Creator.Instantiate();
            dataRoot.Add(Creator.BindedObject);
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
            Creator?.Dispose();
        }
    }
}