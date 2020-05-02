using System.ComponentModel.Composition;
using System.Windows.Media;
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
    public class ComponentIconDescriptor : IDefaultIconDescriptorModule<IGlyphComponent>
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
        static public readonly Brush ToolCategoryBrush = Brushes.Black;
        
        public IconDescription GetDefaultIcon(IGlyphComponent glyphComponent)
        {
            switch (glyphComponent)
            {
                case IGlyphContainer _:
                    return new IconDescription(PackIconMaterialKind.HexagonMultiple, CoreCategoryBrush);
                default:
                    return new IconDescription(PackIconMaterialKind.Hexagon, CoreCategoryBrush);
            }
        }

        public IconDescription GetIcon(IGlyphComponent glyphComponent)
        {
            switch (glyphComponent)
            {
                case ISceneNode _:
                    return new IconDescription(PackIconMaterialKind.AxisArrow, SceneGraphCategoryBrush);
                case ICamera _:
                    return new IconDescription(PackIconMaterialKind.Video, SceneGraphCategoryBrush);
                case IView _:
                    return new IconDescription(PackIconMaterialKind.Monitor, SceneGraphCategoryBrush);
                    
                case IAnimationPlayer _:
                    return new IconDescription(PackIconMaterialKind.Animation, AnimationCategoryBrush);
                case IAnimationGraph _:
                    return new IconDescription(PackIconMaterialKind.LockPattern, AnimationCategoryBrush);
                case Motion _:
                    return new IconDescription(PackIconMaterialKind.RunFast, AnimationCategoryBrush);
                case MotorBase _:
                    return new IconDescription(PackIconMaterialKind.Engine, AnimationCategoryBrush);

                case ISpriteSheet _:
                    return new IconDescription(PackIconMaterialKind.ImageMultiple, GraphicsCategoryBrush);
                case ISpriteSource _:
                    return new IconDescription(PackIconMaterialKind.Image, GraphicsCategoryBrush);
                case SpriteTransformer _:
                    return new IconDescription(PackIconMaterialKind.Compare, GraphicsCategoryBrush);
                case FillingRectangle _:
                    return new IconDescription(PackIconMaterialKind.CameraMeteringMatrix, GraphicsCategoryBrush);
                case PrimitiveRenderer _:
                    return new IconDescription(PackIconMaterialKind.Shape, GraphicsCategoryBrush);
                case RendererBase _:
                    return new IconDescription(PackIconMaterialKind.ProjectorScreen, GraphicsCategoryBrush);
                    
                case SoundLoader _:
                    return new IconDescription(PackIconMaterialKind.MusicBox, AudioCategoryBrush);
                case SoundEmitter _:
                    return new IconDescription(PackIconMaterialKind.VolumeHigh, AudioCategoryBrush);
                case SoundListener _:
                    return new IconDescription(PackIconMaterialKind.Headphones, AudioCategoryBrush);
                    
                case RectangleCollider _:
                    return new IconDescription(PackIconMaterialKind.VectorRectangle, PhysicsCategoryBrush);
                case CircleCollider _:
                    return new IconDescription(PackIconMaterialKind.VectorCircleVariant, PhysicsCategoryBrush);
                case IGridCollider _:
                    return new IconDescription(PackIconMaterialKind.ViewGridOutline, PhysicsCategoryBrush);
                case ICollider _:
                    return new IconDescription(PackIconMaterialKind.VectorPolygon, PhysicsCategoryBrush);

                case InteractiveRoot _:
                    return new IconDescription(PackIconMaterialKind.Layers, InputCategoryBrush);
                case Controls _:
                    return new IconDescription(PackIconMaterialKind.GestureTap, InputCategoryBrush);
                    
                case Actor _:
                    return new IconDescription(PackIconMaterialKind.AccountBox, ScriptingCategoryBrush);
                case Trigger _:
                    return new IconDescription(PackIconMaterialKind.AlertBoxOutline, ScriptingCategoryBrush);
                    
                case InterfaceRoot _:
                    return new IconDescription(PackIconMaterialKind.ViewDashboardVariant, UiCategoryBrush);
                case UserInterface _:
                    return new IconDescription(PackIconMaterialKind.ViewDashboard, UiCategoryBrush);
                    
                case Flipper _:
                    return new IconDescription(PackIconMaterialKind.FlipHorizontal, CoreCategoryBrush);
                case ILayerRoot _:
                    return new IconDescription(PackIconMaterialKind.Layers, CoreCategoryBrush);
                case ILayerManager _:
                    return new IconDescription(PackIconMaterialKind.LayersTriple, CoreCategoryBrush);
                    
                case FreeCamera _:
                    return new IconDescription(PackIconMaterialKind.VideoSwitch, ToolCategoryBrush);
                case TransformationEditor _:
                    return new IconDescription(PackIconMaterialKind.AxisArrow, ToolCategoryBrush);

                default:
                    return IconDescription.None;
            }
        }
    }
}