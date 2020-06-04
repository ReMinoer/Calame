using System.ComponentModel.Composition;
using System.Windows.Media;
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
    public class GlyphTypesIconDescriptor : IIconDescriptorModule, IDefaultIconDescriptorModule
    {
        static public readonly Brush DefaultBrush = Brushes.Black;

        public IconDescription GetIcon(object model)
        {
            switch (model)
            {
                case Vector2 _:
                case Vector3 _:
                case Vector4 _:
                case Point _:
                    return new IconDescription(PackIconMaterialKind.VectorLine, DefaultBrush);

                case Segment _:
                    return new IconDescription(PackIconMaterialKind.VectorLine, DefaultBrush);
                case Triangle _:
                    return new IconDescription(PackIconMaterialKind.VectorTriangle, DefaultBrush);
                case Rectangle _:
                case TopLeftRectangle _:
                case CenteredRectangle _:
                case Quad _:
                    return new IconDescription(PackIconMaterialKind.VectorRectangle, DefaultBrush);
                case Circle _:
                    return new IconDescription(PackIconMaterialKind.VectorCircleVariant, DefaultBrush);

                case Matrix _:
                case Matrix3X3 _:
                    return new IconDescription(PackIconMaterialKind.Matrix, DefaultBrush);
                case Quaternion _:
                    return new IconDescription(PackIconMaterialKind.RotateOrbit, DefaultBrush);

                case Color _:
                    return new IconDescription(PackIconMaterialKind.Palette, DefaultBrush);

                case FilePath _:
                    return new IconDescription(PackIconMaterialKind.FileOutline, DefaultBrush);
                case FolderPath _:
                    return new IconDescription(PackIconMaterialKind.FolderOpen, DefaultBrush);

                default: return IconDescription.None;
            }
        }

        public IconDescription GetDefaultIcon(object model)
        {
            switch (model)
            {
                case IArray _:
                    return new IconDescription(PackIconMaterialKind.Grid, DefaultBrush);
                case ISpace _:
                    return new IconDescription(PackIconMaterialKind.ChartScatterPlot, DefaultBrush);

                case IShape _:
                    return new IconDescription(PackIconMaterialKind.VectorPolygon, DefaultBrush);
                case IArea _:
                    return new IconDescription(PackIconMaterialKind.TextureBox, DefaultBrush);

                default: return IconDescription.None;
            }
        }
    }
}