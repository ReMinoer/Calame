﻿using Calame.Demo.Data.Engine;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Core.Modelization;
using Glyph.IO;
using Glyph.Tools.Transforming;
using Microsoft.Xna.Framework;
using Simulacra.Binding;
using Simulacra.Injection.Binding;
using Simulacra.IO.Binding;

namespace Calame.Demo.Data.Data
{
    public interface IInstanceData<out T> : IGlyphCreator<T>, IPositionController
        where T : IGlyphComponent
    {
        Vector2 LocalPosition { get; set; }
    }

    public abstract class InstanceDataBase<TData, T> : BindedData<TData, T>, IInstanceData<T>
        where TData : InstanceDataBase<TData, T>
        where T : class, IInstanceObject
    {
        public Vector2 LocalPosition { get; set; }

        static InstanceDataBase()
        {
            Bindings.From(x => x.LocalPosition).To(x => x.SceneNode.LocalPosition);
        }

        Vector2 IPositionController.Position
        {
            get => LocalPosition;
            set => LocalPosition = value;
        }
    }

    public class SpriteInstanceData : InstanceDataBase<SpriteInstanceData, SpriteInstanceObject>
    {
        public FilePath AssetPath { get; set; }

        static SpriteInstanceData()
        {
            Bindings.FromPath(x => x.AssetPath).To(x => x.SpriteLoader.AssetPath);
        }
    }

    public class FileInstanceData : InstanceDataBase<FileInstanceData, InstanceObject>
    {
        public FilePath FilePath { get; set; }

        static FileInstanceData()
        {
            Bindings.FromPath(x => x.FilePath)
                .Load(() => new DataContractSerializationFormat<IGlyphCreator<IGlyphComponent>>())
                .CreateComponent()
                .SetToBindedObject();
        }
    }
}