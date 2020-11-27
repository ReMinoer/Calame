using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Diese;
using Glyph.IO;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using MahApps.Metro.IconPacks;
using Microsoft.Xna.Framework;
using Simulacra.Utils;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    public class GlyphTypesIconDescriptor : TypeHybridIconDescriptorModuleBase
    {
        static public readonly Brush DefaultBrush = Brushes.Black;

        public override bool Handle(Type type)
        {
            string typeNamespace = type?.Namespace;
            if (typeNamespace == null)
                return false;

            return typeNamespace.StartsWith(nameof(Microsoft.Xna.Framework))
                || typeNamespace.StartsWith(nameof(Glyph))
                || typeNamespace.StartsWith(nameof(Simulacra));
        }

        public override IconDescription GetTypeIcon(Type type)
        {
            if (type.Is<Vector2>()
                || type.Is<Vector3>()
                || type.Is<Vector4>()
                || type.Is<Point>())
                return new IconDescription(PackIconMaterialKind.VectorLine, DefaultBrush);

            if (type.Is<Segment>())
                return new IconDescription(PackIconMaterialKind.VectorLine, DefaultBrush);
            if (type.Is<Triangle>())
                return new IconDescription(PackIconMaterialKind.VectorTriangle, DefaultBrush);
            if (type.Is<Rectangle>()
                || type.Is<TopLeftRectangle>()
                || type.Is<CenteredRectangle>()
                || type.Is<Quad>())
                return new IconDescription(PackIconMaterialKind.VectorRectangle, DefaultBrush);
            if (type.Is<Circle>())
                return new IconDescription(PackIconMaterialKind.VectorCircleVariant, DefaultBrush);

            if (type.Is<Matrix>()
                || type.Is<Matrix3X3>())
                return new IconDescription(PackIconMaterialKind.Matrix, DefaultBrush);
            if (type.Is<Quaternion>())
                return new IconDescription(PackIconMaterialKind.RotateOrbit, DefaultBrush);

            if (type.Is<Color>())
                return new IconDescription(PackIconMaterialKind.Palette, DefaultBrush);

            if (type.Is<FilePath>())
                return new IconDescription(PackIconMaterialKind.FileOutline, DefaultBrush);
            if (type.Is<FolderPath>())
                return new IconDescription(PackIconMaterialKind.FolderOpen, DefaultBrush);

            return IconDescription.None;
        }

        public override IconDescription GetTypeDefaultIcon(Type type)
        {
            if (type.Is<IArray>())
                return new IconDescription(PackIconMaterialKind.Grid, DefaultBrush);
            if (type.Is<ISpace>())
                return new IconDescription(PackIconMaterialKind.ChartScatterPlot, DefaultBrush);

            if (type.Is<IShape>())
                return new IconDescription(PackIconMaterialKind.VectorPolygon, DefaultBrush);
            if (type.Is<IArea>())
                return new IconDescription(PackIconMaterialKind.TextureBox, DefaultBrush);

            return IconDescription.None;
        }
    }
}