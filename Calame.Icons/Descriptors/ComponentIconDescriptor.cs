using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Diese;
using Glyph;
using Glyph.Animation;
using Glyph.Animation.Motors.Base;
using Glyph.Audio;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Colliders;
using Glyph.Core.Inputs;
using Glyph.Core.Layers;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Renderer.Base;
using Glyph.Scripting;
using Glyph.Tools;
using Glyph.Tools.Transforming;
using Glyph.UI;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IGlyphComponent>))]
    [Export(typeof(IDefaultIconDescriptorModule<IGlyphComponent>))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule<IGlyphComponent>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<IGlyphComponent>))]
    public class ComponentIconDescriptor : TypeHybridIconDescriptorModuleBase<IGlyphComponent>
    {
        static public readonly Brush CoreCategoryBrush = Brushes.DimGray;
        static public readonly Brush SceneGraphCategoryBrush = Brushes.RoyalBlue;
        static public readonly Brush AnimationCategoryBrush = Brushes.DarkMagenta;
        static public readonly Brush GraphicsCategoryBrush = Brushes.DarkGreen;
        static public readonly Brush AudioCategoryBrush = Brushes.DarkRed;
        static public readonly Brush PhysicsCategoryBrush = Brushes.SaddleBrown;
        static public readonly Brush InputCategoryBrush = Brushes.DeepPink;
        static public readonly Brush ScriptingCategoryBrush = Brushes.DarkOrange;
        static public readonly Brush UiCategoryBrush = Brushes.Goldenrod;
        static public readonly Brush ToolCategoryBrush = IconBrushes.Default;

        public override IconDescription GetTypeDefaultIcon(Type type)
        {
            if (type.Is<IGlyphContainer>())
                return new IconDescription(PackIconMaterialKind.HexagonMultiple, CoreCategoryBrush);
            if (type.Is<IGlyphComponent>())
                return new IconDescription(PackIconMaterialKind.Hexagon, CoreCategoryBrush);

            return IconDescription.None;
        }

        public override IconDescription GetTypeIcon(Type type)
        {
            if (type.Is<ISceneNode>())
                return new IconDescription(PackIconMaterialKind.AxisArrow, SceneGraphCategoryBrush);
            if (type.Is<ICamera>())
                return new IconDescription(PackIconMaterialKind.Video, SceneGraphCategoryBrush);
            if (type.Is<IView>())
                return new IconDescription(PackIconMaterialKind.Monitor, SceneGraphCategoryBrush);

            if (type.Is<IAnimationPlayer>())
                return new IconDescription(PackIconMaterialKind.Animation, AnimationCategoryBrush);
            if (type.Is<IAnimationGraph>())
                return new IconDescription(PackIconMaterialKind.LockPattern, AnimationCategoryBrush);
            if (type.Is<Motion>())
                return new IconDescription(PackIconMaterialKind.RunFast, AnimationCategoryBrush);
            if (type.Is<MotorBase>())
                return new IconDescription(PackIconMaterialKind.Engine, AnimationCategoryBrush);

            if (type.Is<ISpriteSheet>())
                return new IconDescription(PackIconMaterialKind.ImageMultiple, GraphicsCategoryBrush);
            if (type.Is<ISpriteSource>())
                return new IconDescription(PackIconMaterialKind.Image, GraphicsCategoryBrush);
            if (type.Is<SpriteTransformer>())
                return new IconDescription(PackIconMaterialKind.Compare, GraphicsCategoryBrush);
            if (type.Is<FillingRectangle>())
                return new IconDescription(PackIconMaterialKind.CameraMeteringMatrix, GraphicsCategoryBrush);
            if (type.Is<MeshRenderer>())
                return new IconDescription(PackIconMaterialKind.Shape, GraphicsCategoryBrush);
            if (type.Is<RendererBase>())
                return new IconDescription(PackIconMaterialKind.ProjectorScreen, GraphicsCategoryBrush);

            if (type.Is<SongPlayer>())
                return new IconDescription(PackIconMaterialKind.Radio, AudioCategoryBrush);
            if (type.Is<SoundLoader>())
                return new IconDescription(PackIconMaterialKind.MusicBox, AudioCategoryBrush);
            if (type.Is<SoundEmitter>())
                return new IconDescription(PackIconMaterialKind.VolumeHigh, AudioCategoryBrush);
            if (type.Is<SoundListener>())
                return new IconDescription(PackIconMaterialKind.Headphones, AudioCategoryBrush);

            if (type.Is<RectangleCollider>())
                return new IconDescription(PackIconMaterialKind.VectorRectangle, PhysicsCategoryBrush);
            if (type.Is<CircleCollider>())
                return new IconDescription(PackIconMaterialKind.VectorCircleVariant, PhysicsCategoryBrush);
            if (type.Is<IGridCollider>())
                return new IconDescription(PackIconMaterialKind.ViewGridOutline, PhysicsCategoryBrush);
            if (type.Is<ICollider>())
                return new IconDescription(PackIconMaterialKind.VectorPolygon, PhysicsCategoryBrush);

            if (type.Is<InteractiveRoot>())
                return new IconDescription(PackIconMaterialKind.Layers, InputCategoryBrush);
            if (type.Is<Controls>())
                return new IconDescription(PackIconMaterialKind.GestureTap, InputCategoryBrush);

            if (type.Is<Actor>())
                return new IconDescription(PackIconMaterialKind.AccountBox, ScriptingCategoryBrush);
            if (type.Is<Trigger>())
                return new IconDescription(PackIconMaterialKind.AlertBoxOutline, ScriptingCategoryBrush);

            if (type.Is<InterfaceRoot>())
                return new IconDescription(PackIconMaterialKind.ViewDashboardVariant, UiCategoryBrush);
            if (type.Is<UserInterface>())
                return new IconDescription(PackIconMaterialKind.ViewDashboard, UiCategoryBrush);

            if (type.Is<Flipper>())
                return new IconDescription(PackIconMaterialKind.FlipHorizontal, CoreCategoryBrush);
            if (type.Is<ILayerRoot>())
                return new IconDescription(PackIconMaterialKind.Layers, CoreCategoryBrush);
            if (type.Is<ILayerManager>())
                return new IconDescription(PackIconMaterialKind.LayersTriple, CoreCategoryBrush);

            if (type.Is<FreeCamera>())
                return new IconDescription(PackIconMaterialKind.VideoSwitch, ToolCategoryBrush);
            if (type.Is<TransformationEditor>())
                return new IconDescription(PackIconMaterialKind.AxisArrow, ToolCategoryBrush);

            return IconDescription.None;
        }
    }
}