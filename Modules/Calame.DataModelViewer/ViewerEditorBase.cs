using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Engine;
using Glyph.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.DataModelViewer
{
    public abstract class ViewerEditorBase<T> : IEditor
        where T : IGlyphCreator
    {
        private Viewport _viewport;
        public string DisplayName => SaveLoadFormat.FileType.DisplayName;
        public IEnumerable<string> FileExtensions => SaveLoadFormat.FileType.Extensions;
        public abstract string ContentPath { get; }

        protected abstract ISaveLoadFormat<T> SaveLoadFormat { get; }
        public abstract Task<IGlyphCreator> NewDataAsync();

        public virtual IGlyphComposite<IGlyphComponent> PrepareEditor(GlyphEngine engine)
        {
            var pixel = new Texture2D(engine.Injector.Resolve<Func<GraphicsDevice>>()(), 1, 1);
            pixel.SetData(new[] { Color.White });

            engine.Root.Schedulers.Draw.Plan(drawer =>
            {
                drawer.GraphicsDevice.SetRenderTarget(drawer.DefaultRenderTarget);
                _viewport = drawer.GraphicsDevice.Viewport;

                float targetAspectRatio = VirtualResolution.AspectRatio;
                
                int width = _viewport.Width;
                int height = (int)(width / targetAspectRatio + .5f);

                if (height > _viewport.Height)
                {
                    height = _viewport.Height;
                    width = (int)(height * targetAspectRatio + .5f);
                }

                var viewport = new Viewport
                {
                    X = _viewport.Width / 2 - width / 2,
                    Y = _viewport.Height / 2 - height / 2,
                    Width = width,
                    Height = height,
                    MinDepth = 0,
                    MaxDepth = 1
                };

                drawer.GraphicsDevice.Viewport = viewport;

                drawer.SpriteBatchStack.Push(SpriteBatchContext.Default);
                drawer.SpriteBatchStack.Current.Draw(pixel, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.CornflowerBlue);
                drawer.SpriteBatchStack.Pop();

                var drawSceneContext = new SpriteBatchContext
                {
                    SpriteSortMode = SpriteSortMode.Deferred,
                    TransformMatrix = drawer.ViewMatrix * drawer.ResolutionMatrix
                };

                drawer.SpriteBatchStack.Push(drawSceneContext);

            }).AtStart();

            engine.Root.Schedulers.Draw.Plan(drawer =>
            {
                drawer.SpriteBatchStack.Pop();
                drawer.GraphicsDevice.Viewport = _viewport;
            }).AtEnd();

            return engine.Root;
        }

        public async Task<IGlyphCreator> LoadDataAsync(Stream stream)
        {
            return SaveLoadFormat.Load(stream);
        }
        
        public async Task SaveDataAsync(object obj, Stream stream)
        {
            SaveLoadFormat.Save((T)obj, stream);
        }
    }
}