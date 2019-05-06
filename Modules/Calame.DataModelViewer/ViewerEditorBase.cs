using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Glyph;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Engine;
using Glyph.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Calame.DataModelViewer
{
    public abstract class ViewerEditorBase<T> : IEditor
        where T : IGlyphCreator
    {
        public string DisplayName => SaveLoadFormat.FileType.DisplayName;
        public IEnumerable<string> FileExtensions => SaveLoadFormat.FileType.Extensions;
        public abstract string ContentPath { get; }

        protected abstract ISaveLoadFormat<T> SaveLoadFormat { get; }
        public abstract Task<IGlyphCreator> NewDataAsync();

        public virtual IGlyphComposite<IGlyphComponent> PrepareEditor(GlyphEngine engine, GlyphObject editorRoot)
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

            return dataRoot;
        }

        public virtual Task<IGlyphCreator> LoadDataAsync(Stream stream)
        {
            return Task.Run<IGlyphCreator>(() => SaveLoadFormat.Load(stream));
        }
        
        public virtual Task SaveDataAsync(object obj, Stream stream)
        {
            return Task.Run(() => SaveLoadFormat.Save((T)obj, stream));
        }
    }
}