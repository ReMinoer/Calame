using System;
using System.ComponentModel.DataAnnotations;
using Calame.Demo.Data.Engine;
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
    public interface IInstanceData<out T> : IGlyphCreator<T>, IPositionController, IRotationController, IScaleController
        where T : IGlyphComponent
    {
        Vector2 LocalPosition { get; set; }
        float LocalRotation { get; set; }
        float LocalScale { get; set; }
    }

    public abstract class InstanceDataBase<TData, T> : BindedData<TData, T>, IInstanceData<T>
        where TData : InstanceDataBase<TData, T>
        where T : class, IInstanceObject
    {
        public Vector2 LocalPosition { get; set; }
        public float LocalRotation { get; set; }

        [Range(0, float.MaxValue)]
        public float LocalScale { get; set; } = 1;

        static InstanceDataBase()
        {
            Bindings.From(x => x.LocalPosition).To(x => x.SceneNode.LocalPosition);
            Bindings.From(x => x.LocalRotation).To(x => x.SceneNode.LocalRotation);
            Bindings.From(x => x.LocalScale).To(x => x.SceneNode.LocalScale);
        }

        Vector2 IPositionController.Position
        {
            get => LocalPosition;
            set => LocalPosition = value;
        }

        float IRotationController.Rotation
        {
            get => LocalRotation;
            set => LocalRotation = value;
        }

        float IScaleController.Scale
        {
            get => LocalScale;
            set => LocalScale = value;
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
        [FileType(".circle", ".rectangle", ".scene", DisplayName = "Data Files")]
        [RootFolder(Environment.SpecialFolder.MyDocuments)]
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