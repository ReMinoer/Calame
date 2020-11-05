using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Demo.Data.Data;
using Calame.Icons;
using Calame.Icons.Base;
using Diese;
using Glyph.Composition.Modelization;
using MahApps.Metro.IconPacks;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IGlyphData>))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule<IGlyphData>))]
    public class DataIconDescriptor : TypeIconDescriptorModuleBase<IGlyphData>
    {
        static public readonly Brush DefaultBrush = Brushes.DimGray;

        public override IconDescription GetTypeIcon(Type type)
        {
            if (type.Is<SceneData>())
                return new IconDescription(PackIconMaterialKind.Group, DefaultBrush);
            if (type.Is<RectangleData>())
                return new IconDescription(PackIconMaterialKind.VectorRectangle, DefaultBrush);
            if (type.Is<CircleData>())
                return new IconDescription(PackIconMaterialKind.VectorCircleVariant, DefaultBrush);
            if (type.Is<FileInstanceData>())
                return new IconDescription(PackIconMaterialKind.FileCode, DefaultBrush);
            if (type.Is<SpriteInstanceData>())
                return new IconDescription(PackIconMaterialKind.Image, DefaultBrush);
            
            return IconDescription.None;
        }
    }
}