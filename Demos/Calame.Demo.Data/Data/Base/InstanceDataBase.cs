using System.ComponentModel.DataAnnotations;
using Calame.Demo.Data.Engine;
using Glyph.Composition.Modelization;
using Glyph.Tools.Transforming;
using Microsoft.Xna.Framework;
using Simulacra.Binding;
using Simulacra.Injection.Binding;

namespace Calame.Demo.Data.Data.Base
{
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
        
        bool IPositionController.IsLocalPosition => true;
        Vector2 IPositionController.Position
        {
            get => LocalPosition;
            set => LocalPosition = value;
        }
        Vector2 IPositionController.LivePosition
        {
            get => LocalPosition;
            set => LocalPosition = value;
        }
        
        bool IRotationController.IsLocalRotation => true;
        float IRotationController.Rotation
        {
            get => LocalRotation;
            set => LocalRotation = value;
        }
        float IRotationController.LiveRotation
        {
            get => LocalRotation;
            set => LocalRotation = value;
        }
        
        bool IScaleController.IsLocalScale => true;
        float IScaleController.Scale
        {
            get => LocalScale;
            set => LocalScale = value;
        }
        float IScaleController.LiveScale
        {
            get => LocalScale;
            set => LocalScale = value;
        }
    }
}