using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.DataModelViewer;
using Calame.Icons;
using Calame.Icons.Base;
using Diese;
using MahApps.Metro.IconPacks;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IEditorSource>))]
    [Export(typeof(ITypeIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule<IEditorSource>))]
    public class EditorIconDescriptor : TypeIconDescriptorModuleBase<IEditorSource>
    {
        static public readonly Brush DefaultBrush = Brushes.DimGray;

        public override IconDescription GetTypeIcon(Type type)
        {
            if (type.Is<SceneEditorSource>())
                return new IconDescription(PackIconMaterialKind.Group, DefaultBrush);
            if (type.Is<RectangleEditorSource>())
                return new IconDescription(PackIconMaterialKind.VectorRectangle, DefaultBrush);
            if (type.Is<CircleEditorSource>())
                return new IconDescription(PackIconMaterialKind.VectorCircleVariant, DefaultBrush);

            return IconDescription.None;
        }
    }
}