using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Demo.Data.Data;
using Calame.Icons;
using Calame.Icons.Base;
using Glyph.Composition.Modelization;
using MahApps.Metro.IconPacks;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IGlyphData>))]
    public class DataIconDescriptor : IconDescriptorModuleBase<IGlyphData>
    {
        static public readonly Brush DefaultBrush = Brushes.DimGray;

        public override IconDescription GetIcon(IGlyphData model)
        {
            switch (model)
            {
                case SceneData _: return new IconDescription(PackIconMaterialKind.Group, DefaultBrush);
                case RectangleData _: return new IconDescription(PackIconMaterialKind.VectorRectangle, DefaultBrush);
                case CircleData _: return new IconDescription(PackIconMaterialKind.VectorCircleVariant, DefaultBrush);
                case FileInstanceData _: return new IconDescription(PackIconMaterialKind.FileCode, DefaultBrush);
                case SpriteInstanceData _: return new IconDescription(PackIconMaterialKind.Image, DefaultBrush);
                default: return IconDescription.None;
            }
        }
    }
}